using Bangk.Common.Dto;
using Bangk.Domain.Contract.Integrations;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.BackgroundTask
{
    public class RetryIntegrationsJob : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly CrontabSchedule _schedule;

        public RetryIntegrationsJob(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _schedule = CrontabSchedule.Parse(_configuration.GetSection("RetryIntegrationsJob").Value);
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
                var scheduleIntegrationRepository = scope.ServiceProvider.GetRequiredService<IScheduleIntegrationRepository>();
                var process = await scheduleIntegrationRepository.ScheduleIntegrationByDate(DateTime.UtcNow);
                await ProcessScheduleIntegration(process);
            }
        }

        private async Task ProcessScheduleIntegration(List<ScheduleIntegrationDto> scheduleIntegrationDtos)
        {
            foreach (var scheduleIntegration in scheduleIntegrationDtos)
            {
                if (scheduleIntegration.IntegrationType == 1)
                {
                    await TrySBSIntegration(scheduleIntegration);
                }
                else
                {
                    await TrySarlaftIntegration(scheduleIntegration);
                }
            }
        }

        private async Task TrySBSIntegration(ScheduleIntegrationDto schedule)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var validationSBSService = scope.ServiceProvider.GetRequiredService<IValidationSBSService>();
                await validationSBSService.Execute(schedule.LoanApplicationId);
            }
        }

        private async Task TrySarlaftIntegration(ScheduleIntegrationDto schedule)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var validationSarlaftService = scope.ServiceProvider.GetRequiredService<IValidationSarlaftService>();
                await validationSarlaftService.Execute(schedule.LoanApplicationId);
            }
        }
    }
}
