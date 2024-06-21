using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanApplicationStateHistory
{
    public interface ILoanApplicationStateHistory
    {
        Task<LoanApplicationStateHistoryResultDto> Execute(int loanApplicationId);
    }
}
