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
                var nextOccurrence = _schedule.GetNextOccurrence(DateTime.UtcNow);
                var delay = nextOccurrence - DateTime.UtcNow;

                if (delay.TotalMilliseconds > 0)
                    await Task.Delay(delay, stoppingToken);

                await Execute();
            }
        }

        private async Task Execute()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                var notifications = await notificationRepository.GetNotificationByDate(DateTime.UtcNow);
                await ProcessNotification(notifications);
            }
        }

        private async Task ProcessNotification(List<NotificationDto> notifications)
        {
            foreach (var notificationDto in notifications)
            {
                if (notificationDto.NotificationType == 1)
                {
                    await SendNotification(notificationDto);
                }
                else
                {
                    await ChangeLoanApplicationState(notificationDto);
                }
            }
        }

        private async Task SendNotification(NotificationDto notificationDto)
        {
            var url = _configuration.GetSection("LoanInsurancePolicy:UrlFront").Value;
            var sendEmail = new SendGridEmailDto
            {
                email = notificationDto.ClientEmail,
                From = _configuration.GetSection("SendGridEmailSender:From").Value,
                Key = _configuration.GetSection("SendGridEmailSender:Key").Value,
                Name = _configuration.GetSection("SendGridEmailSender:Name").Value,
                TemplateId = _configuration.GetSection("SendGridEmailSender:RememberCompleteApplicationEmail").Value,
                Data = new
                {
                    ClientName = $"{notificationDto.ClientName + " " + notificationDto.ClientLastName}",
                    UrlCallback = url + $"?loanApplicationId={notificationDto.ApplicationId}"
                }
            };

            await SendGridEmailSender.SendEmailAsync(sendEmail);
        }

        private async Task ChangeLoanApplicationState(NotificationDto notificationDto)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {

                using TransactionScope transactionScope = new(TransactionScopeAsyncFlowOption.Enabled);
                var loanApplicationStateHistoryRepository = scope.ServiceProvider.GetRequiredService<ILoanApplicationStateHistoryRepository>();
                var clientRepository = scope.ServiceProvider.GetRequiredService<IClientRepository>();
                var loanApplicationHistory = new Repositories.Entities.LoanApplicationStateHistory
                {
                    CreationDate = DateTime.UtcNow,
                    LoanApplicationId = notificationDto.ApplicationId,
                    Observations = string.Empty,
                    StateId = (int)ApplicationStateHistoryEnum.CotizaciónNoRespondida
                };

                await loanApplicationStateHistoryRepository.Create(loanApplicationHistory);
                var client = await clientRepository.GetClientByLoanApplicationId(notificationDto.ApplicationId);
                client.State = (int)ClientStateEnum.PerdidoRechazo;
                await clientRepository.Update(client);

                var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                var notifications = await notificationRepository.GetNotificationByApplicationId(notificationDto.ApplicationId);
                if (notifications != null && notifications.Count > 0)
                {
                    await notificationRepository.DeleteBulk(notifications);
                }

                transactionScope.Complete();
            }
        }
    }
}
