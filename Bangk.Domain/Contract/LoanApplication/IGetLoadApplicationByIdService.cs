using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanApplication
{
    public interface IGetLoadApplicationByIdService
    {
        Task<LoanApplicationInfoDto> Execute(int loanApplicationId);
    }
}
