using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface ILoanInsurancePolicyRepository : IGenericRepository<LoanInsurancePolicy, LoanInsurancePolicyDto>
    {
        Task<LoanInsurancePolicyDto> GetAllLoanInsurancePolicyByLoanApplicationId(int loanApplicationId);
        Task<LoanInsurancePolicy> GetLoanInsurancePolicyByLoadApplicationId(int loanApplicationId);
        Task<ClientSBSInfoDto> GetClientSBSInfoDtoyByLoanApplicationId(int loanApplicationId);
        Task<ClientSarlaftInfoDto> GetClientSarlaftInfoDtoyByLoanApplicationId(int loanApplicationId);
        int GetLastConsecutive();
    }
}
