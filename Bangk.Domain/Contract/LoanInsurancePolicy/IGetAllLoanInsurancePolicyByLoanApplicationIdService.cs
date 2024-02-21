using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanQuote
{
    public interface IGetAllLoanInsurancePolicyByLoanApplicationIdService
    {
        Task<LoanInsurancePolicyDto> Execute(int loanApplicationId);
    }
}
