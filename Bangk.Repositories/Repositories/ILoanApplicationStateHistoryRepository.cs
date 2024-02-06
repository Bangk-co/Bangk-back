using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Linq;

namespace Bangk.Repositories.Repositories
{
    public interface ILoanApplicationStateHistoryRepository : IGenericRepository<LoanApplicationStateHistory, LoanApplicationStateHistoryDto>
    {
        IQueryable<LoanApplicationStateHistoryDto> GetLoanApplicationStateHistory (int loanApplicationId);
    }
}
