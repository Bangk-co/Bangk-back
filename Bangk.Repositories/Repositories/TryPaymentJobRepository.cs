using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class TryPaymentJobRepository : GenericRepository<TryPaymentJob, TryPaymentJobDto>, ITryPaymentJobRepository
    {
        public TryPaymentJobRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<List<TryPaymentJobDto>> GetryPaymentJobByCountFail(int countFailId)
        {
            var query = await Context.TryPaymentJobs.Where(t => t.CountFailed == countFailId).Select(t => new TryPaymentJobDto
            {
                CountFailed = t.CountFailed,
                ApplicationReference = t.ApplicationReference,

            }).ToListAsync();

            return query;
        }

        public async Task<TryPaymentJob> GetryPaymentJobByReference(string applicationReference)
        {
            return await Context.TryPaymentJobs.FirstOrDefaultAsync(t => t.ApplicationReference == applicationReference);
        }
    }
}
