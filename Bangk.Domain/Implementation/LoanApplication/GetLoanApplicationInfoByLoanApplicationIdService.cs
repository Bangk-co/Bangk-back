using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplication;
using Bangk.Repositories.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class GetLoanApplicationInfoByLoanApplicationIdService : IGetLoanApplicationInfoByLoanApplicationIdService
    {

        private readonly ILoanApplicationRepository _ILoanApplicationRepository;

        public GetLoanApplicationInfoByLoanApplicationIdService(ILoanApplicationRepository LoanApplicationRepository)
        {
            _ILoanApplicationRepository = LoanApplicationRepository;
        }

        public async Task<List<LoanApplicationDto>> Execute(int loanApplicationId)
        {
            var loanApplications = await _ILoanApplicationRepository.GetLoanApplicationInfoByLoanApplicationId(loanApplicationId);
            return  loanApplications;
        }
    }
}
