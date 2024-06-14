using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplication;
using Bangk.Repositories.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class GetLoanApplicationServices : ILoanApplicationServices
    {

        private readonly ILoanApplicationRepository _ILoanApplicationRepository;

        public GetLoanApplicationServices(ILoanApplicationRepository LoanApplicationRepository)
        {
            _ILoanApplicationRepository = LoanApplicationRepository;
        }

        public async Task<List<LoanApplicationDto>> Execute(int ClientId)
        {
            var loanApplications = await _ILoanApplicationRepository.GetLoanApplicationInfo(ClientId);
            return  loanApplications;
        }
    }
}
