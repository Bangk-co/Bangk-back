using Bangk.Domain.Contract.ScheduleIntegration;
using Bangk.Repositories.Repositories;
using System;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.ScheduleIntegration
{
    public class ScheduleIntegrationService : IScheduleIntegrationService
    {
        private readonly IScheduleIntegrationRepository _scheduleIntegrationRepository;
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        public ScheduleIntegrationService(IScheduleIntegrationRepository scheduleIntegrationRepository, ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository)
        {
            _scheduleIntegrationRepository = scheduleIntegrationRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
        }

        public async Task CreateNextTrySBSOrSarlaft(int loanApplicationId, int typeValidation)
        {
            var scheduler = await _scheduleIntegrationRepository.GetScheduleIntegrationByLoanApplicationId(loanApplicationId, typeValidation);
            if (scheduler == null)
            {
                var nextExecution = new Repositories.Entities.ScheduleIntegration
                {
                    CountFails = 1,
                    NextTryDate = DateTime.UtcNow.AddMinutes(1),
                    IntegrationType = typeValidation,
                    LoanApplicationId = loanApplicationId
                };

                await _scheduleIntegrationRepository.Create(nextExecution);
            }
            else
            {
                if (scheduler.CountFails < 3)
                {
                    scheduler.NextTryDate = DateTime.UtcNow.AddMinutes(scheduler.CountFails == 1 ? 30 : 60);
                    scheduler.CountFails += 1;
                    await _scheduleIntegrationRepository.Update(scheduler);
                }
            }
        }
    }
}
