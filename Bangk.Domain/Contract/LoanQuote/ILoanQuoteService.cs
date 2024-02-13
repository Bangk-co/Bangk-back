using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanQuote
{
    public interface ILoanQuoteService
    {
        Task<int> Execute(int insuranceId, int loanApplicationId, long yearlyFee, string insuranceName);
    }
}
