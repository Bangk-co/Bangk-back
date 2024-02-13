using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Common.Utils;
using Bangk.Domain.Contract.LoanQuote;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            ILoanApplicationRepository LoanApplicationRepository, IClientRepository clientRepository, ISettingRepository settingRepository )
        {
            _loanQuoteRepository = loanQuoteRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _configuration = configuration;
            _loanApplicationRepository = LoanApplicationRepository;
            _clientRepository = clientRepository;
            _settingRepository = settingRepository;
        }

        public async Task<int> Execute(int insuranceId, int loanApplicationId, long yearlyFee)
        {
            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);


            //var insurance = _settingRepository.GetById(insuranceId);
            var loanQuote = new Repositories.Entities.LoanQuote
            {
                InsuranceId = insuranceId,
                LoanApplicationId = loanApplicationId,
                YearlyFee = yearlyFee,

            };

            await _loanQuoteRepository.Create(loanQuote);

            

            var loanApplicationStatusHistory = new Repositories.Entities.LoanApplicationStateHistory
            {
                LoanApplicationId = loanApplicationId,
                Observations = $"Cotizado enviada",
                StateId = (int)ApplicationStateHistoryEnum.CotizacionEnviada

            };

            await _loanApplicationStateHistoryRepository.Create(loanApplicationStatusHistory);

            var loanApplicationInfo = _loanApplicationRepository.GetById(loanApplicationId);
            var clientInfo = _clientRepository.GetById(loanApplicationInfo.Result.ClientId);

            var url = _configuration.GetSection("LoanQuote:UrlFront").Value + $"?requestId={loanApplicationInfo.Result.Id}";

            var sendEmail = new SendGridEmailDto
            {
                email = clientInfo.Result.Email,
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
