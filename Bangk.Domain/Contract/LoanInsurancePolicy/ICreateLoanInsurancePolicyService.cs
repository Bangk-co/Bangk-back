using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanInsurancePolicy
{
    public interface ICreateLoanInsurancePolicyService 
    {
        Task<ResponseDto<bool>> Execute(LoanInsurancePolicySaveDto loanInsurancePolicySaveDto);
    }
}
