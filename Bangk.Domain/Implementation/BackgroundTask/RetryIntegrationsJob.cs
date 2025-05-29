using Bangk.Common;
using Bangk.Common.Dto;
using Bangk.Domain.Contract.Integrations;
using Bangk.Repositories.Repositories;
using Coravel.Invocable;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.BackgroundTask
{
    public class RetryIntegrationsJob : IInvocable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RetryIntegrationsJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Invoke()
        {
            await Execute();
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
