using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanForm;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanForm
{
    public class GetLoanFormService: IGetLoanFormService
    {

        private readonly ILoanFormRepository _loanFormRepository;

        public GetLoanFormService(ILoanFormRepository loanFormRepository)
        {
            _loanFormRepository = loanFormRepository;
        }

        public async Task<List<LoanFormDto>> Execute(int loanApplicationId)
        {
            return await _loanFormRepository.GetLoanFormById(loanApplicationId);
        }
    }
}
