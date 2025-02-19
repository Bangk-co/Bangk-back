using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.Clientify;
using Bangk.Domain.Contract.Integrations;
using Bangk.Domain.Contract.ScheduleIntegration;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using ValidacionOFAC;
//using ValidationOFACTest;

namespace Bangk.Domain.Implementation.Integrations
{
    public class ValidationSBSService : IValidationSBSService
    {
        private readonly IConfiguration _configuration;
        private readonly IClientRepository _clientRepository;
        private readonly ILoanInsurancePolicyRepository _loanInsurancePolicyRepository;
        private readonly IValidationSBSLogRepository _validationSBSLogRepository;
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        private readonly IValidationSarlaftService _validationSarlaftService;
        private readonly IScheduleIntegrationService _scheduleIntegrationService;
        private readonly IScheduleIntegrationRepository _scheduleIntegrationRepository;
        private readonly IChangeStateDealService _changeStateDealService;

        public ValidationSBSService(IConfiguration configuration, ILoanInsurancePolicyRepository loanInsurancePolicyRepository,
           IClientRepository clientRepository, IValidationSBSLogRepository validationSBSLogRepository,
           ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository, IValidationSarlaftService validationSarlaftService,
           IScheduleIntegrationService scheduleIntegrationService, IScheduleIntegrationRepository scheduleIntegrationRepository,
            IChangeStateDealService changeStateDealService)
        {
            _configuration = configuration;
            _clientRepository = clientRepository;
            _loanInsurancePolicyRepository = loanInsurancePolicyRepository;
            _validationSBSLogRepository = validationSBSLogRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _validationSarlaftService = validationSarlaftService;
            _scheduleIntegrationService = scheduleIntegrationService;
            _scheduleIntegrationRepository = scheduleIntegrationRepository;
            _changeStateDealService = changeStateDealService;
        }

        public async Task Execute(int loanApplicationId, bool isCallFromApi = false)
        {
            try
            {
                //ValidacionOFACClient clientOFAC = new ValidacionOFACClient(ValidacionOFACClient.EndpointConfiguration.BasicHttpsBinding_IValidacionOFAC);
                ValidacionOFACClient clientOFAC = new ValidacionOFACClient(ValidacionOFACClient.EndpointConfiguration.BasicHttpsBinding_IValidacionOFAC);
                var client = await _clientRepository.GetClientByLoanApplicationId(loanApplicationId);
                var clientSBSInf = await _loanInsurancePolicyRepository.GetClientSBSInfoDtoyByLoanApplicationId(loanApplicationId);
                var responseValidation = await clientOFAC.ValidarOFACAsync(_configuration.GetSection("IntegrationSBS:Token").Value, _configuration.GetSection("IntegrationSBS:ApplicationName").Value,
                    client.DocumenIdentificationTypeId == 76 ? TipoDocumento.Cedula : TipoDocumento.Cedula_Extranjeria, client.IdentificationNumber, client.Name, client.LastName,
                    clientSBSInf.BirthCountrySBSId, clientSBSInf.CountryNationalitySBSId);

                await SaveValidacionOFACLog(responseValidation);
                if (responseValidation.RegistroValidado)
                {
                    if (responseValidation.TipoRestriccionOfac == 2)//no apto
                    {
                        await DeleteRetryJob(loanApplicationId);
                        await SendPolicyNotIssueNotification(client);
                        await SaveLoanHistory(loanApplicationId, "Cliente no apto para emisión de póliza", (int)ApplicationStateHistoryEnum.EstudioListaRestrictivaSBS, responseValidation.TipoRestriccionOfac);
                        await SaveLoanHistory(loanApplicationId, null, (int)ApplicationStateHistoryEnum.ClienteNoAsegurable, null);
                        client.State = (int)ClientStateEnum.NoAsegurable;
                        await _clientRepository.Update(client);
                        await _changeStateDealService.Execute((int)StateClientifyDeal.Lost, loanApplicationId);

                    }
                    else if (responseValidation.TipoRestriccionOfac == 1)//presenta novedad
                    {
                        await DeleteRetryJob(loanApplicationId);
                        await SaveLoanHistory(loanApplicationId, "Presenta novedad en lista restrictiva", (int)ApplicationStateHistoryEnum.EstudioListaRestrictivaSBS, responseValidation.TipoRestriccionOfac);
                    }
                    else//apto
                    {
                        await DeleteRetryJob(loanApplicationId);
                        await SaveLoanHistory(loanApplicationId, "Cliente apto en lista restrictiva", (int)ApplicationStateHistoryEnum.EstudioListaRestrictivaSBS, responseValidation.TipoRestriccionOfac);
                        await _validationSarlaftService.Execute(loanApplicationId);
                    }
                }
                else
                {
                    await SaveLoanHistory(loanApplicationId, responseValidation.DescripcionError, (int)ApplicationStateHistoryEnum.EstudioListaRestrictivaSBS, isCallFromApi ? 4 : 3);
                    await _scheduleIntegrationService.CreateNextTrySBSOrSarlaft(loanApplicationId, 1);
                }
            }
            catch (Exception ex)
            {
                await SaveLoanHistory(loanApplicationId, ex.Message, (int)ApplicationStateHistoryEnum.EstudioListaRestrictivaSBS, isCallFromApi ? 4 : 3);
                await _scheduleIntegrationService.CreateNextTrySBSOrSarlaft(loanApplicationId, 1);
            }
        }

        private async Task SaveValidacionOFACLog(clsResultadoValidacion clsResultadoValidacion)
        {
            var validationOfac = new ValidationSBSLog
            {
                DetalleIdsErrores = clsResultadoValidacion.DetalleIdsErrores,
                DescripcionError = clsResultadoValidacion.DescripcionError,
                DetalleListaOfac = clsResultadoValidacion.DetalleListaOFAC,
                OrigenRestriccionOfac = clsResultadoValidacion.OrigenRestriccionOfac,
                OrigenRestriccionOfacDesc = clsResultadoValidacion.OrigenRestriccionOfacDesc,
                RegistroValidado = clsResultadoValidacion.RegistroValidado,
                TipoRestriccionOfac = clsResultadoValidacion.TipoRestriccionOfac,
                TipoRestriccionOfacDesc = clsResultadoValidacion.TipoRestriccionOfacDesc
            };

            await _validationSBSLogRepository.Create(validationOfac);
        }

        private async Task SendPolicyNotIssueNotification(Repositories.Entities.Client clientInfo)
        {
            var url = _configuration.GetSection("PolicyNotIssued:UrlFront").Value;
            var sendEmail = new SendGridEmailDto
            {
                email = clientInfo.Email,
                From = _configuration.GetSection("SendGridEmailSender:From").Value,
                Key = _configuration.GetSection("SendGridEmailSender:Key").Value,
                Name = _configuration.GetSection("SendGridEmailSender:Name").Value,
                TemplateId = _configuration.GetSection("SendGridEmailSender:PolicyNotIssuedEmail").Value,
                Data = new
                {
                    clientname = $"{clientInfo.Name + " " + clientInfo.LastName}",
                    UrlCallback = url
                }
            };

            await SendGridEmailSender.SendEmailAsync(sendEmail);
        }

        private async Task SaveLoanHistory(int loanApplicationId, string observation, int state, int? validationResultCode)
        {
            var loanApplicationHistory = new Repositories.Entities.LoanApplicationStateHistory
            {
                CreationDate = DateTime.UtcNow,
                LoanApplicationId = loanApplicationId,
                Observations = observation ?? string.Empty,
                StateId = state,
                ValidationClientResult = validationResultCode
            };

            await _loanApplicationStateHistoryRepository.Create(loanApplicationHistory);
        }

        private async Task DeleteRetryJob(int loanApplicationId)
        {
            var scheduler = await _scheduleIntegrationRepository.GetScheduleIntegrationByLoanApplicationId(loanApplicationId, 1);
            if(scheduler != null)
            {
                await _scheduleIntegrationRepository.Delete(scheduler);
            }

        }
    }
}
