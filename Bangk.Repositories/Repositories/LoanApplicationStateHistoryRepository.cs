using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;

namespace Bangk.Repositories.Repositories
{
    public class LoanApplicationStateHistoryRepository : GenericRepository<LoanApplicationStateHistory, LoanApplicationStateHistoryDto>, ILoanApplicationStateHistoryRepository
    {
        public LoanApplicationStateHistoryRepository(AppDbContext context) : base(context)
        {
        }
    }
}
