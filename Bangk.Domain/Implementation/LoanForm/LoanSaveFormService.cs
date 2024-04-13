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
    public class LoanSaveFormService: ILoanSaveFormService
    {
        private readonly ILoanFormRepository _loanFormRepository;

        public LoanSaveFormService(ILoanFormRepository loanFormRepository)
        {
            _loanFormRepository = loanFormRepository;
        }

        public async Task Execute(int loanApplicationId, string formName, string concept, string comment)
        {

            var loanForm = new Repositories.Entities.LoanForm
            {
                LoanApplicationId = loanApplicationId,
                FormName = formName,
                Concept = concept,
                Comment = comment

            };

            await _loanFormRepository.Create(loanForm);
            
        }
    }
}
