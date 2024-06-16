using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanQuote
{
    public interface IValidateLoanApplicationQouteService
    {
        Task<string> Execute(int loanApplicationId);
    }
}
