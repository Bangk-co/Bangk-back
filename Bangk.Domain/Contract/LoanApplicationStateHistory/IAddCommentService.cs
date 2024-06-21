using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanApplicationStateHistory
{
    public interface IAddCommentService
    {
        Task<ResponseDto<bool>> Execute(int loanApplicationHistoryId, string comment);
    }
}
