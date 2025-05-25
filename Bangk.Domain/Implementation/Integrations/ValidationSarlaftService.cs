using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.Clientify;
using Bangk.Domain.Contract.Integrations;
using Bangk.Domain.Contract.ScheduleIntegration;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
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
        private readonly IChangeStatusDealService _changeStatusDealService;
        private readonly IChangeStateDealService _changeStateDealService;

        public ValidationSarlaftService(ILoanInsurancePolicyRepository loanInsurancePolicyRepository, IConfiguration configuration, IClientRepository clientRepository,
            ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository, IScheduleIntegrationService scheduleIntegrationService,
            IValidationSarlaftRepository validationSarlaftRepository,
            IScheduleIntegrationRepository scheduleIntegrationRepository,
            IChangeStatusDealService changeStatusDealService,
            IChangeStateDealService changeStateDealService)
        {
            _loanInsurancePolicyRepository = loanInsurancePolicyRepository;
            _configuration = configuration;
            _clientRepository = clientRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _scheduleIntegrationService = scheduleIntegrationService;
            _validationSarlaftRepository = validationSarlaftRepository;
            _scheduleIntegrationRepository = scheduleIntegrationRepository;
            _changeStatusDealService = changeStatusDealService;
            _changeStateDealService = changeStateDealService;
        }

        public async Task Execute(int loanApplicationId, bool isCallFromApi = false)
        {
            var client = await _clientRepository.GetClientByLoanApplicationId(loanApplicationId);
            var clientSarlaftInf = await _loanInsurancePolicyRepository.GetClientSarlaftInfoDtoyByLoanApplicationId(loanApplicationId);
            string apiUrl = _configuration.GetSection("Sarlaft:Url").Value;
            var listParameter = new List<SarlaftParametersDto>();
            var parameter = new SarlaftParametersDto
            {
                TipoConsulta = _configuration.GetSection("Sarlaft:TipoConsulta").Value,
                TipoDocumento = client.DocumenIdentificationTypeId == 76 ? "1" : "2",
                NumeroDocumento = client.IdentificationNumber,
                DigitoVerificacion = string.Empty,
                FechaExpedicion = client.DocumentIssueDate.HasValue ? client.DocumentIssueDate.Value.ToString("yyyyMMdd") : string.Empty,
                NumeroCelular = client.PhoneNumber,
                EsPPE = clientSarlaftInf.IsPublicPerson ? "1" : "0",
                VinculosPPE = clientSarlaftInf.HasBondPublicPerson ? "1" : "0",
                ValorAsegurado = clientSarlaftInf.LoanBalance,
                ValorPrimaKMs = (long)clientSarlaftInf.YearlyFee,
                Intermediario = long.Parse(_configuration.GetSection("Sarlaft:Intermediario").Value),
                TipoParticipante = string.Empty,
                VinculoLaboral = string.Empty,
                Sponsor = _configuration.GetSection("Sarlaft:Sponsor").Value
            };

            listParameter.Add(parameter);
            string jsonBody = JsonConvert.SerializeObject(listParameter);
            var byteArray = new UTF8Encoding().GetBytes($"{_configuration.GetSection("Sarlaft:User").Value}:{_configuration.GetSection("Sarlaft:Password").Value}");
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);
                    if (response.IsSuccessStatusCode )
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        await CreateValidationSarlaftLog(loanApplicationId, responseBody);
                        dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject[]>(responseBody)[0];
                        if (string.Equals(jsonObject.estadoSarlaft.estado, "V"))//apto
                        {
                            await SaveLoanHistory(loanApplicationId, $"Cliente apto para emisión de póliza. Descripción: {jsonObject.estadoSarlaft.descripcion}", (int)ApplicationStateHistoryEnum.EstudioSarlaft, 0);
                            await SaveLoanHistory(loanApplicationId, null, (int)ApplicationStateHistoryEnum.SolicitudEmisionPoliza, null);
                            await DeleteRetryJob(loanApplicationId);
                            await _changeStatusDealService.Execute("Emisión", loanApplicationId);
                        }
                        else if (string.Equals(jsonObject.estadoSarlaft.estado, "F") || string.Equals(jsonObject.estadoSarlaft.estado, "N") ||
                            string.Equals(jsonObject.estadoSarlaft.estado, "P"))//novedad
                        {
                            
                            await SaveLoanHistory(loanApplicationId, $"Presenta novedad en Sarlaft. Descripción: {jsonObject.estadoSarlaft.descripcion}", (int)ApplicationStateHistoryEnum.EstudioSarlaft, 1);
                            await DeleteRetryJob(loanApplicationId);
                        }
                        else if (string.Equals(jsonObject.estadoSarlaft.estado, "R")) //error
                        {
                            await SaveLoanHistory(loanApplicationId, $"Cliente no apto para emisión de póliza. Descripción: {jsonObject.estadoSarlaft.descripcion}", (int)ApplicationStateHistoryEnum.EstudioSarlaft, 2);
                            await SendPolicyNotIssueNotification(client);
                            await SaveLoanHistory(loanApplicationId, null, (int)ApplicationStateHistoryEnum.ClienteNoAsegurable, null);
                            client.State = (int)ClientStateEnum.NoAsegurable;
                            await _clientRepository.Update(client);
                            await DeleteRetryJob(loanApplicationId);
                            await _changeStateDealService.Execute((int)StateClientifyDeal.Lost, loanApplicationId);
                        }
                        else
                        {
                            await SaveLoanHistory(loanApplicationId, $"Descripción: {jsonObject.estadoSarlaft.descripcion}", (int)ApplicationStateHistoryEnum.EstudioSarlaft, isCallFromApi ? 4 : 3);
                            await _scheduleIntegrationService.CreateNextTrySBSOrSarlaft(loanApplicationId, 2);
                        }
                    }
                    else
                    {
                        await SaveLoanHistory(loanApplicationId, $"Descripción: {response.ReasonPhrase}", (int)ApplicationStateHistoryEnum.EstudioSarlaft, isCallFromApi ? 4 : 3);
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
                Email = clientInfo.Email,
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
