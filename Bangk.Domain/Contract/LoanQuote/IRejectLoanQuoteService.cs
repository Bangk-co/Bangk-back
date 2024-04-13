using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanQuote
{
    public interface IRejectLoanQuoteService
    {
        Task<ResponseDto<bool>> Execute(int loanApplicationId, string observation);
    }
}
