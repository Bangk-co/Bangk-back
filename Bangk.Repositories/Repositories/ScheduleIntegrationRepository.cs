using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class ScheduleIntegrationRepository : GenericRepository<ScheduleIntegration, ScheduleIntegrationDto>, IScheduleIntegrationRepository
    {
        public ScheduleIntegrationRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<ScheduleIntegration> GetScheduleIntegrationByLoanApplicationId(int loanApplicationId, int integrationType)
        {
            return await Context.ScheduleIntegrations.Where(s => s.LoanApplicationId == loanApplicationId && s.IntegrationType == integrationType).FirstOrDefaultAsync();
        }
    }
}
