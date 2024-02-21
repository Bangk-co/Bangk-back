using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanQuote;
using Bangk.Repositories.Repositories;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class GetAllLoanInsurancePolicyByLoanApplicationIdService : IGetAllLoanInsurancePolicyByLoanApplicationIdService
    {
        private readonly ILoanInsurancePolicyRepository _loanInsurancePolicyRepository;
        public GetAllLoanInsurancePolicyByLoanApplicationIdService(ILoanInsurancePolicyRepository loanInsurancePolicyRepository)
        {
            _loanInsurancePolicyRepository = loanInsurancePolicyRepository;
        }

        public async Task<LoanInsurancePolicyDto> Execute(int loanApplicationId)
        {
            return await _loanInsurancePolicyRepository.GetAllLoanInsurancePolicyByLoanApplicationId(loanApplicationId);
        }
    }
}
