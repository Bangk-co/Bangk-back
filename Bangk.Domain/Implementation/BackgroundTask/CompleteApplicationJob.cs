using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Bangk.Domain.Implementation.BackgroundTask
{
    public class CompleteApplicationJob : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly CrontabSchedule _schedule;

        public CompleteApplicationJob(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _schedule = CrontabSchedule.Parse(_configuration.GetSection("CompleteApplicationJob").Value);
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var nextOccurrence = _schedule.GetNextOccurrence(DateTime.Now);
                var delay = nextOccurrence - DateTime.Now;

                if (delay.TotalMilliseconds > 0)
                    await Task.Delay(delay, stoppingToken);

                await Execute();
            }
        }

        private async Task Execute()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var loanApplicationStateHistoryRepository = scope.ServiceProvider.GetRequiredService<ILoanApplicationStateHistoryRepository>();
                var loanApplicationList = await loanApplicationStateHistoryRepository.GetLoanApplicationQuoteAccepted();
                var today = DateTime.Today;
                var todayDto = loanApplicationList.Where(dto => dto.CreationDate.Date == today).ToList();
                await SendEmail(todayDto);
                var results3Days = loanApplicationList.Where(dto => (today - dto.CreationDate.Date).TotalDays == 3).ToList();
                await SendEmail(results3Days);
                var results6Days = loanApplicationList.Where(dto => (today - dto.CreationDate.Date).TotalDays == 6).ToList();
                await SendEmail(results6Days);
                var results20PlusDays = loanApplicationList.Where(dto => (today - dto.CreationDate.Date).TotalDays == 20).ToList();
                await SendEmail(results20PlusDays);
                var results30PlusDays = loanApplicationList.Where(dto => (today - dto.CreationDate.Date).TotalDays >= 30).ToList();
                await ChangeLoanApplicationState(results30PlusDays);
            }
        }

        private async Task SendEmail(List<LoanApplicationStateDto> loanApplicationStateDtos)
        {
            var url = _configuration.GetSection("LoanInsurancePolicy:UrlFront").Value;
            foreach (var l in loanApplicationStateDtos)
            {
                var sendEmail = new SendGridEmailDto
                {
                    email = l.ClientEmail,
                    From = _configuration.GetSection("SendGridEmailSender:From").Value,
                    Key = _configuration.GetSection("SendGridEmailSender:Key").Value,
                    Name = _configuration.GetSection("SendGridEmailSender:Name").Value,
                    TemplateId = _configuration.GetSection("SendGridEmailSender:RememberCompleteApplicationEmail").Value,
                    Data = new
                    {
                        ClientName = $"{l.ClientName + " " + l.ClientLastName}",
                        UrlCallback = url + $"?loanApplicationId={l.LoanApplicationId}"
                    }
                };

                await SendGridEmailSender.SendEmailAsync(sendEmail);
            }
        }

        private async Task ChangeLoanApplicationState(List<LoanApplicationStateDto> loanApplicationStateDtos)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var loanApplicationStateHistoryRepository = scope.ServiceProvider.GetRequiredService<ILoanApplicationStateHistoryRepository>();
                var clientRepository = scope.ServiceProvider.GetRequiredService<IClientRepository>();
                using TransactionScope transaction = new(TransactionScopeAsyncFlowOption.Enabled);

                foreach (var l in loanApplicationStateDtos)
                {
                    var loanApplicationHistory = new Repositories.Entities.LoanApplicationStateHistory
                    {
                        CreationDate = DateTime.Now,
                        LoanApplicationId = l.LoanApplicationId,
                        Observations = string.Empty,
                        StateId = (int)ApplicationStateHistoryEnum.CotizaciónNoRespondida
                    };

                    await loanApplicationStateHistoryRepository.Create(loanApplicationHistory);
                    var client = await clientRepository.GetClientByLoanApplicationId(l.LoanApplicationId);
                    client.State = (int)ClientStateEnum.PerdidoRechazo;
                    await clientRepository.Update(client);
                }

                transaction.Complete();
            }
        }
    }
}
