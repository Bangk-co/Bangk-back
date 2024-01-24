using Bangk.Domain.Contract.LoanApplication;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class CreateLoanApplicationService : ICreateLoanApplicationService
    {
        private readonly ILoanApplicationRepository _ILoanApplicationRepository;
        public CreateLoanApplicationService(ILoanApplicationRepository LoanApplicationRepository) 
        {
            _ILoanApplicationRepository = LoanApplicationRepository;
        }
    }
}
