using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanQuote;
using Bangk.Repositories.Repositories;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class GetLoanQuoteInfoByLoanApplicationIdService : IGetLoanQuoteInfoByLoanApplicationIdService
    {
        private readonly ILoanQuoteRepository _loanQuoteRepository;
        public GetLoanQuoteInfoByLoanApplicationIdService(ILoanQuoteRepository loanQuoteRepository)
        {
            _loanQuoteRepository = loanQuoteRepository;
        }

        public async Task<GetLoanQuoteDto> Execute(int loanApplicationId)
        {
            return await _loanQuoteRepository.GetLoanQuoteInfoByLoanApplicationId(loanApplicationId);
        }
    }
}
