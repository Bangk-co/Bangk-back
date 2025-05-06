using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface ITryPaymentJobRepository : IGenericRepository<TryPaymentJob, TryPaymentJobDto>
    {
        Task<List<TryPaymentJobDto>> GetryPaymentJobByCountFail(int countFailId);
        Task<TryPaymentJob> GetryPaymentJobByReference(string applicationReference);
    }
}
