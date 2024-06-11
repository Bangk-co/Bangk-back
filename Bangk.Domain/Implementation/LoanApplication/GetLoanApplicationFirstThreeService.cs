using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplication;
using Bangk.Repositories.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class GetLoanApplicationFirstThreeService : IGetLoanApplicationFirstThreeService
    {

        private readonly ILoanApplicationRepository _ILoanApplicationRepository;

        public GetLoanApplicationFirstThreeService(ILoanApplicationRepository LoanApplicationRepository)
        {
            _ILoanApplicationRepository = LoanApplicationRepository;
        }

        public async Task<List<LoanApplicationDto>> Execute(int ClientId)
        {
            return await _ILoanApplicationRepository.GetLoanApplicationInfo(ClientId, true);
        }
    }
}
