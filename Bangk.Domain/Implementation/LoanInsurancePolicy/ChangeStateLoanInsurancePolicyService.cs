using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.LoanInsurancePolicy;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Bangk.Domain.Implementation.Client
{
    public class ChangeStateLoanInsurancePolicyService : IChangeStateLoanInsurancePolicyService
    {
        private readonly ILoanInsurancePolicyRepository _loanInsurancePolicyRepository;
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IConfiguration _configuration;
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ISettingRepository _settingRepository;

        public ChangeStateLoanInsurancePolicyService(ILoanInsurancePolicyRepository loanInsurancePolicyRepository, INotificationRepository notificationRepository,
            ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository, IConfiguration configuration, ILoanApplicationRepository loanApplicationRepository, 
            IClientRepository clientRepository, ISettingRepository settingRepository)
        {
            _loanInsurancePolicyRepository = loanInsurancePolicyRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _notificationRepository = notificationRepository;
            _configuration = configuration;
            _loanApplicationRepository = loanApplicationRepository;
            _clientRepository = clientRepository;
            _settingRepository = settingRepository;
        }

        public async Task<ResponseDto<bool>> Execute(int loanApplicationId)
        {
            var result = new ResponseDto<bool>
            {
                Result = true
            };

            var loanInsurencePolicy = await _loanInsurancePolicyRepository.GetLoanInsurancePolicyByLoadApplicationId(loanApplicationId);
            if (loanInsurencePolicy == null)
            {
                result.HasErrors = true;
                result.Errors.Add($"Aplicacion Id no existe.");
                result.Result = false;
                return result;
            }

            var observation = string.Empty;
            if (string.IsNullOrEmpty(loanInsurencePolicy.Diseases))
            {
               observation = $"Riesgo de salud presente {loanInsurencePolicy.DiseasesMoreInfo}"; 
            }

            if (loanInsurencePolicy.Disability.HasValue && loanInsurencePolicy.Disability.Value)
            {
                observation += !string.IsNullOrEmpty(observation) ?  $",Riesgo de salud presente {loanInsurencePolicy.DisabilityMoreInfo}" : $"Riesgo de salud presente {loanInsurencePolicy.DisabilityMoreInfo}";
            }

            if (loanInsurencePolicy.PlannedMedicalProcidure.HasValue && loanInsurencePolicy.PlannedMedicalProcidure.Value)
            {
                observation += !string.IsNullOrEmpty(observation) ? $",Riesgo de salud presente {loanInsurencePolicy.PlannedMedicalProcidureMoreInfo}" : $"Riesgo de salud presente {loanInsurencePolicy.PlannedMedicalProcidureMoreInfo}";
            }

            var loadApplicationStateHistory = new Repositories.Entities.LoanApplicationStateHistory
            {
                LoanApplicationId = loanApplicationId,
                Observations = observation,
                StateId = (int)ApplicationStateHistoryEnum.FormulariosDiligenciados,
            };

            await _loanApplicationStateHistoryRepository.Create(loadApplicationStateHistory);


            var hasHealthInfo = !string.IsNullOrEmpty(loanInsurencePolicy.DisabilityMoreInfo) ||
            !string.IsNullOrEmpty(loanInsurencePolicy.Diseases) ||
            !string.IsNullOrEmpty(loanInsurencePolicy.DiseasesMoreInfo) ||
            loanInsurencePolicy.Weight.HasValue ||
            loanInsurencePolicy.Height.HasValue ||
            loanInsurencePolicy.HIVActive.HasValue ||
            loanInsurencePolicy.PlannedMedicalProcidure.HasValue ||
            loanInsurencePolicy.Disability.HasValue;

            if (hasHealthInfo)
            {
                var imc = (loanInsurencePolicy.Weight ?? 0) / Math.Pow((double)(loanInsurencePolicy.Height ?? 1), 2);
                var vih = loanInsurencePolicy.HIVActive;
                var questionHealth1 = loanInsurencePolicy.PlannedMedicalProcidure;
                var questionHealth2 = loanInsurencePolicy.Disability;
                var plannedMedicalProcidureMoreInfo = loanInsurencePolicy.PlannedMedicalProcidureMoreInfo;
                var disabilityMoreInfo = loanInsurencePolicy.DisabilityMoreInfo;
                var imcLower = float.Parse((await _settingRepository.GetById((int)(float)IMCEnum.ImcLower)).Code);
                var imcHigher = float.Parse((await _settingRepository.GetById((int)(float)IMCEnum.imcHigher)).Code);

                int trueCount = Convert.ToInt32(vih) + Convert.ToInt32(questionHealth1) + Convert.ToInt32(questionHealth2);

                if (imc <= imcLower || imc >= imcHigher || vih == true || trueCount >= 2)
                {
                    using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

                    var loanApplicationInfo = await _loanApplicationRepository.GetById(loanApplicationId);
                    var clientInfo = _clientRepository.GetById(loanApplicationInfo.ClientId);
                    var url = _configuration.GetSection("PolicyNotIssued:UrlFront").Value;
                    var sendEmail = new SendGridEmailDto
                    {
                        email = clientInfo.Result.Email,
                        From = _configuration.GetSection("SendGridEmailSender:From").Value,
                        Key = _configuration.GetSection("SendGridEmailSender:Key").Value,
                        Name = _configuration.GetSection("SendGridEmailSender:Name").Value,
                        TemplateId = _configuration.GetSection("SendGridEmailSender:PolicyNotIssuedEmail").Value,
                        Data = new
                        {
                            clientname = $"{clientInfo.Result.Name + " " + clientInfo.Result.LastName}",
                            UrlCallback = url
                        }
                    };

                    await SendGridEmailSender.SendEmailAsync(sendEmail);

                    string observations = string.Empty;

                    if (imc <= imcLower || imc >= imcHigher)
                    {
                        observations += "IMC por fuera de lo normal (" + imc.ToString("F4") + "). ";
                    }

                    if (vih.HasValue && vih.Value)
                    {
                        observations += "Presencia de VIH. ";
                    }

                    if (questionHealth1.HasValue && questionHealth1.Value && !string.IsNullOrEmpty(plannedMedicalProcidureMoreInfo))
                    {
                        observations += plannedMedicalProcidureMoreInfo + ".";
                    }

                    if (questionHealth2.HasValue && questionHealth2.Value && !string.IsNullOrEmpty(disabilityMoreInfo))
                    {
                        observations += disabilityMoreInfo + ".";
                    }


                    var loanApplicationHistory = new Repositories.Entities.LoanApplicationStateHistory
                    {
                        CreationDate = DateTime.UtcNow,
                        LoanApplicationId = (int)loanInsurencePolicy.LoanApplicationId,
                        Observations = observations,
                        StateId = (int)ApplicationStateHistoryEnum.ClienteNoAsegurable
                    };

                    await _loanApplicationStateHistoryRepository.Create(loanApplicationHistory);

                    var client = await _clientRepository.GetClientByLoanApplicationId(loanApplicationId);
                    client.State = (int)ClientStateEnum.NoAsegurable;
                    await _clientRepository.Update(client);

                    scope.Complete();

                }

            }

            var notifications = await _notificationRepository.GetNotificationByApplicationId(loanApplicationId);
            if (notifications != null && notifications.Count > 0)
            {
                await _notificationRepository.DeleteBulk(notifications);
            }

            
            return result;
        }
    }
}
