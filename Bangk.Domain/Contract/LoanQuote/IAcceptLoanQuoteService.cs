using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanQuote
{
    public interface IAcceptLoanQuoteService
    {
        Task<ResponseDto<bool>> Execute(int loanApplicationId);
    }
}
