using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanQuote
{
    public interface IGetLoanQuoteInfoByLoanApplicationIdService
    {
        Task<GetLoanQuoteDto> Execute(int loanApplicationId);
    }
}
