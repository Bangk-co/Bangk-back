using Bangk.Common.Dto;
using System;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanQuote
{
    public interface IAcceptLoanQuoteService
    {
        Task<ResponseDto<bool>> Execute(int loanApplicationId, DateTime termsConditionsAcceptanceQuote, DateTime acceptQuoteOtp, DateTime? sbsPrivacyPolicies);
    }
}
