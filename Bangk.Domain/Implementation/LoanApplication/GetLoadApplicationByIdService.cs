using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplication;
using Bangk.Repositories.Repositories;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class GetLoadApplicationByIdService : IGetLoadApplicationByIdService
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        public GetLoadApplicationByIdService(ILoanApplicationRepository LoanApplicationRepository)
        {
            _loanApplicationRepository = LoanApplicationRepository;
        }

        public async Task<LoanApplicationInfoDto> Execute(int loanApplicationId)
        {
            var loanApplications = await _loanApplicationRepository.GetLoanApplicationById(loanApplicationId);
            return loanApplications;
        }
    }
}
