using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface IScheduleIntegrationRepository : IGenericRepository<ScheduleIntegration, ScheduleIntegrationDto>
    {
        Task<ScheduleIntegration> GetScheduleIntegrationByLoanApplicationId(int loanApplicationId, int integrationType);
    }
}
