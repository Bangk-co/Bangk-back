using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
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

        public async Task<List<ScheduleIntegrationDto>> ScheduleIntegrationByDate(DateTime dateToSent)
        {
            var query = from n in Context.ScheduleIntegrations
                        join l in Context.LoanApplication on n.LoanApplicationId equals l.Id
                        join c in Context.Clients on l.ClientId equals c.Id
                        where n.NextTryDate >= dateToSent.AddMinutes(-1) && n.NextTryDate <= dateToSent
                        select new ScheduleIntegrationDto
                        {
                            LoanApplicationId = l.Id,
                            CountFails = n.CountFails,
                            IntegrationType = n.IntegrationType
                        };
            return await query.ToListAsync();
        }
    }
}
