using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanApplicationStateHistory
{
    public interface ILoanApplicationStateHistory
    {
        Task<IQueryable<LoanApplicationStateHistoryDto>> Execute(int loanApplicationId);
    }
}
