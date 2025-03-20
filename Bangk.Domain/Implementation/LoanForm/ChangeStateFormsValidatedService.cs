using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.Integrations;
using Bangk.Domain.Contract.LoanForm;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanForm
{
    public class ChangeStateFormsValidatedService: IChangeStateFormsValidatedService
    {
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IClientRepository _clientRepository;
        private readonly IValidationSBSService _validationSBSService;
        private readonly ILoanQuoteRepository _loanQuoteRepository;

        public ChangeStateFormsValidatedService(ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository, ILoanApplicationRepository loanApplicationRepository, 
            IUserRepository userRepository, IConfiguration configuration, IClientRepository clientRepository, IValidationSBSService validationSBSService, ILoanQuoteRepository loanQuoteRepository)
        {
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _loanApplicationRepository = loanApplicationRepository;
            _userRepository = userRepository;
            _configuration = configuration;
            _clientRepository = clientRepository;
            _validationSBSService = validationSBSService;
            _loanQuoteRepository = loanQuoteRepository;
        }

        public async Task<ResponseDto<bool>> Execute(int loanApplicationId, string observation)
        {
            var loanApplication = _loanApplicationRepository.GetById(loanApplicationId);
            var result = new ResponseDto<bool>
            {
                Result = true
            };

            if (loanApplication == null)
            {
                result.HasErrors = true;
                result.Errors.Add($"La Applicacion ingresado no existe");
                result.Result = false;
                return result;
            }

            var loanApplicationHistory = new Repositories.Entities.LoanApplicationStateHistory
            {
                CreationDate = DateTime.Now,
                LoanApplicationId = loanApplicationId,
                Observations = observation ?? string.Empty,
                StateId = (int)ApplicationStateHistoryEnum.FormulariosValidados
            };

            await _loanApplicationStateHistoryRepository.Create(loanApplicationHistory);
            //var userWithClientId = _userRepository.GetByClientId(loanApplication.Result.ClientId);
            //var client = _clientRepository.GetById(loanApplication.Result.ClientId);
            //if (userWithClientId == null)
            //{
            //    var url = _configuration.GetSection("CreateClientAccount:UrlFront").Value + $"?requestId={client.Result.Id}&email={client.Result.Email}";
            //    var sendEmail = new SendGridEmailDto
            //    {
            //        email = client.Result.Email,
            //        From = _configuration.GetSection("SendGridEmailSender:From").Value,
            //        Key = _configuration.GetSection("SendGridEmailSender:Key").Value,
            //        Name = _configuration.GetSection("SendGridEmailSender:Name").Value,
            //        TemplateId = _configuration.GetSection("SendGridEmailSender:CreateClientAccountEmail").Value,
            //        Data = new
            //        {
            //            clientname = $"{client.Result.Name}",
            //            UrlCallback = url
            //        }
            //    };

            //    await SendGridEmailSender.SendEmailAsync(sendEmail);
            //}

            var loanQupte = await _loanQuoteRepository.GetLoanQuoteByLoadApplicationId(loanApplicationId);
            if (loanQupte.InsuranceId == (int)InsuranceEnum.SBS)
            {
                await _validationSBSService.Execute(loanApplicationId);
            }
            
            return result;
        }
    }
}
