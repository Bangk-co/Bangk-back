using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanInsurancePolicy
{
    public interface IChangeStateLoanInsurancePolicyService
    {
        Task<ResponseDto<bool>> Execute(int loanApplicationId);
    }
}
