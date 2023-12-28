using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Login
{
    public interface IResetPasswordService
    {
        Task<ResponseDto<bool>> Execute(ResetPasswordDto resetPasswordDto);
    }
}
