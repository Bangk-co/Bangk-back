using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface ILoanApplicationStateHistoryRepository : IGenericRepository<LoanApplicationStateHistory, LoanApplicationStateHistoryDto>
    {
        IQueryable<LoanApplicationStateHistoryDto> GetLoanApplicationStateHistory (int loanApplicationId);
        Task<LoanApplicationStateHistory> GetLoanApplicationStateHistoryByStateAndLoanApplicationId(int stateId, int loanApplicationId);
        Task<List<LoanApplicationStateDto>> GetLoanApplicationQuoteAccepted();
    }
}
