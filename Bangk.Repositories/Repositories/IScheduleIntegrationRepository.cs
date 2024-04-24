using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface IScheduleIntegrationRepository : IGenericRepository<ScheduleIntegration, ScheduleIntegrationDto>
    {
        Task<ScheduleIntegration> GetScheduleIntegrationByLoanApplicationId(int loanApplicationId, int integrationType);
        Task<List<ScheduleIntegrationDto>> ScheduleIntegrationByDate(DateTime dateToSent);
    }
}
