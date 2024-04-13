using Bangk.Domain.Contract.LoanApplication;
using Bangk.Repositories.Repositories;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class CreateLoanApplicationService : ICreateLoanApplicationService
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        public CreateLoanApplicationService(ILoanApplicationRepository LoanApplicationRepository) 
        {
            _loanApplicationRepository = LoanApplicationRepository;
        }
    }
}
