using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.Integrations;
using Bangk.Domain.Contract.ScheduleIntegration;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Integrations
{
    public class ValidationSarlaftService : IValidationSarlaftService
    {
        private readonly ILoanInsurancePolicyRepository _loanInsurancePolicyRepository;
        private readonly IConfiguration _configuration;
        private readonly IClientRepository _clientRepository;
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        private readonly IScheduleIntegrationService _scheduleIntegrationService;
        private readonly IValidationSarlaftRepository _validationSarlaftRepository;
        private readonly IScheduleIntegrationRepository _scheduleIntegrationRepository;

        public ValidationSarlaftService(ILoanInsurancePolicyRepository loanInsurancePolicyRepository, IConfiguration configuration, IClientRepository clientRepository,
            ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository, IScheduleIntegrationService scheduleIntegrationService,
            IValidationSarlaftRepository validationSarlaftRepository, IScheduleIntegrationRepository scheduleIntegrationRepository)
        {
            _loanInsurancePolicyRepository = loanInsurancePolicyRepository;
            _configuration = configuration;
            _clientRepository = clientRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _scheduleIntegrationService = scheduleIntegrationService;
            _validationSarlaftRepository = validationSarlaftRepository;
            _scheduleIntegrationRepository = scheduleIntegrationRepository;
        }

        public async Task Execute(int loanApplicationId, bool isCallFromApi = false)
        {
            var client = await _clientRepository.GetClientByLoanApplicationId(loanApplicationId);
            var clientSarlaftInf = await _loanInsurancePolicyRepository.GetClientSarlaftInfoDtoyByLoanApplicationId(loanApplicationId);
            string apiUrl = _configuration.GetSection("Sarlaft:Url").Value;
            var parameter = new SarlaftParametersDto
            {
                DigitoVerificacion = string.Empty,
                EsPPE = clientSarlaftInf.IsPublicPerson,
                FechaExpedicion = client.DocumentIssueDate.HasValue ? client.DocumentIssueDate.Value.ToString("yyyyMMdd") : string.Empty,
                NumeroCelular = client.PhoneNumber,
                NumeroDocumento = client.IdentificationNumber,
                PrimaKMs = clientSarlaftInf.YearlyFee,
                TipoConsulta = "1",
                TipoDocumento = client.DocumenIdentificationTypeId == 76 ? "1" : "2",
                TipoParticipante = string.Empty,
                ValorAsegurado = clientSarlaftInf.LoanBalance,
                VinculoLaboral = string.Empty,
                VinculosPPE = clientSarlaftInf.HasBondPublicPerson,
                Intermediario = _configuration.GetSection("Sarlaft:Intermediario").Value,
                Ramo = string.Empty,
                Sponsor = string.Empty
            };
            string jsonBody = JsonConvert.SerializeObject(parameter);
            var byteArray = new UTF8Encoding().GetBytes($"{_configuration.GetSection("Sarlaft:User").Value}:{_configuration.GetSection("Sarlaft:Password").Value}");
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    await CreateValidationSarlaftLog(loanApplicationId, responseBody);
                    if (response.IsSuccessStatusCode)
                    {

                        dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject[]>(responseBody)[0];
                        if (string.Equals(jsonObject.estadoSarlaft.estado, "V"))//apto
                        {
                            await SaveLoanHistory(loanApplicationId, "Cliente apto para emisión de póliza", (int)ApplicationStateHistoryEnum.EstudioSarlaft, 0);
                            await SaveLoanHistory(loanApplicationId, null, (int)ApplicationStateHistoryEnum.SolicitudEmisionPoliza, null);
                            await DeleteRetryJob(loanApplicationId);
                        }
                        else if (string.Equals(jsonObject.estadoSarlaft.estado, "F") || string.Equals(jsonObject.estadoSarlaft.estado, "N") ||
                            string.Equals(jsonObject.estadoSarlaft.estado, "P"))//novedad
                        {
                            await SaveLoanHistory(loanApplicationId, "Presenta novedad en lista restrictiva", (int)ApplicationStateHistoryEnum.EstudioSarlaft, 1);
                            await DeleteRetryJob(loanApplicationId);
                        }
                        else if (string.Equals(jsonObject.estadoSarlaft.estado, "R")) //error
                        {
                            await SaveLoanHistory(loanApplicationId, "Cliente no apto para emisión de póliza", (int)ApplicationStateHistoryEnum.EstudioSarlaft, 2);
                            await SendPolicyNotIssueNotification(client);
                            await DeleteRetryJob(loanApplicationId);
                        }
                        else
                        {
                            await SaveLoanHistory(loanApplicationId, jsonObject.descripcion, (int)ApplicationStateHistoryEnum.EstudioSarlaft, isCallFromApi ? 4 : 3);
                            await _scheduleIntegrationService.CreateNextTrySBSOrSarlaft(loanApplicationId, 2);
                        }
                    }
                    else
                    {
                        await SaveLoanHistory(loanApplicationId, response.ReasonPhrase, (int)ApplicationStateHistoryEnum.EstudioSarlaft, isCallFromApi ? 4 : 3);
                        await _scheduleIntegrationService.CreateNextTrySBSOrSarlaft(loanApplicationId, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                await SaveLoanHistory(loanApplicationId, ex.Message, (int)ApplicationStateHistoryEnum.EstudioSarlaft, isCallFromApi ? 4 : 3);
                await _scheduleIntegrationService.CreateNextTrySBSOrSarlaft(loanApplicationId, 2);
            }
        }

        private async Task CreateValidationSarlaftLog(int loanApplicationId, string contentResponse)
        {
            var response = new ValidationSarlaftLog
            {
                CreationDate = DateTime.UtcNow,
                JsonResponse = contentResponse,
                LoanApplicationId = loanApplicationId,
            };

            await _validationSarlaftRepository.Create(response);
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
        private async Task DeleteRetryJob(int loanApplicationId)
        {
            var scheduler = await _scheduleIntegrationRepository.GetScheduleIntegrationByLoanApplicationId(loanApplicationId, 2);
            if (scheduler != null)
            {
                await _scheduleIntegrationRepository.Delete(scheduler);
            }

        }
    }
}
