using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanInsurancePolicy
{
    public interface ICreateLoanInsurancePolicyService 
    {
        Task<ResponseDto<LoanInsurancePolicyDto>> Execute(LoanInsurancePolicySaveDto loanInsurancePolicySaveDto);
    }
}
