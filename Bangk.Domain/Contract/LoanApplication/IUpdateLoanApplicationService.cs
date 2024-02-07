using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanApplication
{
    public interface IUpdateLoanApplicationService
    {
        Task<ResponseDto<bool>> Execute(UpdateLoanApplicationDto updateLoanApplicationDto);
    }
}
