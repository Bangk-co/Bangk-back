using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanInsurancePolicy
{
    public interface ICreateLoanInsurancePolicyService 
    {
        Task<ResponseDto<LoanInsurancePolicyDto>> Execute(LoanInsurancePolicySaveDto loanInsurancePolicySaveDto);
    }
}
