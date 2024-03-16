using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanForm
{
    public interface IChangeStateFormsValidatedService
    {
        Task<ResponseDto<bool>> Execute(int loanApplicationId, string observation);
    }
}
