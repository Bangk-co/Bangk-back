using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface ILoanApplicationRepository : IGenericRepository<LoanApplication, LoanApplicationDto>
    {
        List<LoanApplicationDto> GetLoanApplicationInfo(int ClientId);
        Task<LoanApplicationInfoDto> GetLoanApplicationById(int loanApplicationId);
    }
}
