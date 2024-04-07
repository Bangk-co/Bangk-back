using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface ILoanQuoteRepository : IGenericRepository<LoanQuote, LoanQuoteDto>
    {
        Task<GetLoanQuoteDto> GetLoanQuoteInfoByLoanApplicationId(int loanApplicationId);
        Task<LoanQuote> GetLoanQuoteByLoadApplicationId(int loanApplicationId);
    }
}
