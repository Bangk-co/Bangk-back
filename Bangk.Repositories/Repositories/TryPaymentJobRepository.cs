using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;

namespace Bangk.Repositories.Repositories
{
    public class TryPaymentJobRepository : GenericRepository<TryPaymentJob, TryPaymentJobDto>, ITryPaymentJobRepository
    {
        public TryPaymentJobRepository(AppDbContext context) : base(context)
        {

        }
    }
}
