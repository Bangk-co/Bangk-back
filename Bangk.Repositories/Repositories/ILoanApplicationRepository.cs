using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface ILoanApplicationRepository : IGenericRepository<LoanApplication, LoanApplicationDto>
    {
        Task<List<LoanApplicationDto>> GetLoanApplicationInfo(int ClientId, bool takeFirstThree = false);
        Task<LoanApplicationInfoDto> GetLoanApplicationById(int loanApplicationId);
        Task<List<LoanApplicationDto>> GetLoanApplicationInfoByLoanApplicationId(int loanApplicationId);
    }
}
