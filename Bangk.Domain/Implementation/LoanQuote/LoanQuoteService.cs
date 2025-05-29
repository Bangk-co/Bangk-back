using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.LoanQuote;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Transactions;

namespace Bangk.Domain.Implementation.LoanQuote
{
    public class LoanQuoteService : ILoanQuoteService
    {
        private readonly ILoanQuoteRepository _loanQuoteRepository;
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        private readonly IConfiguration _configuration;
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ISettingRepository _settingRepository;

        public LoanQuoteService(ILoanQuoteRepository loanQuoteRepository, ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository, IConfiguration configuration,
            ILoanApplicationRepository LoanApplicationRepository, IClientRepository clientRepository, ISettingRepository settingRepository)
        {
            _loanQuoteRepository = loanQuoteRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _configuration = configuration;
            _loanApplicationRepository = LoanApplicationRepository;
            _clientRepository = clientRepository;
            _settingRepository = settingRepository;
        }

        public async Task<int> Execute(int insuranceId, int loanApplicationId, decimal yearlyFee, string insuranceName)
        {
            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);
            var loanQuote = new Repositories.Entities.LoanQuote
            {
                InsuranceId = insuranceId,
                LoanApplicationId = loanApplicationId,
                YearlyFee = yearlyFee
            };

            await _loanQuoteRepository.Create(loanQuote);
            var loanApplicationInfo =  await _loanApplicationRepository.GetById(loanApplicationId);
            var loanApplicationStatusHistory = new Repositories.Entities.LoanApplicationStateHistory
            {
                LoanApplicationId = loanApplicationId,
                Observations = $"Cotizado con {insuranceName}",
                StateId = (int)ApplicationStateHistoryEnum.CotizacionEnviada
            };

            var smlv = int.Parse((await _settingRepository.GetById((int)SMLVEnum.SMLV)).Code);
            var smlvNumber = loanApplicationInfo.LoanBalance / smlv;
            if (smlvNumber > 135)
            {
                loanApplicationStatusHistory.Observations += ",Valor del préstamo supera los 135 SMMLV";
            }

            await _loanApplicationStateHistoryRepository.Create(loanApplicationStatusHistory);
            var clientInfo = _clientRepository.GetById(loanApplicationInfo.ClientId);
            ////var url = _configuration.GetSection("ValidateLoanStateQuote").Value + $"?loanApplicationId={loanApplicationInfo.Id}";
            var url = _configuration.GetSection("LoanQuote:UrlFront").Value + $"?requestId={loanApplicationInfo.Id}&email={clientInfo.Result.Email}";
            var sendEmail = new SendGridEmailDto
            {
                Email = clientInfo.Result.Email,
                From = _configuration.GetSection("SendGridEmailSender:From").Value,
                Key = _configuration.GetSection("SendGridEmailSender:Key").Value,
                Name = _configuration.GetSection("SendGridEmailSender:Name").Value,
                TemplateId = _configuration.GetSection("SendGridEmailSender:ShippingQuoteEmail").Value,
                Data = new
                {
                    clientname = $"{clientInfo.Result.Name +" "+ clientInfo.Result.LastName}",
                    UrlCallback = url
                }
            };

            await SendGridEmailSender.SendEmailAsync(sendEmail);
            scope.Complete();
            return loanApplicationId;
        }
    }
}
