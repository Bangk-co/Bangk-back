using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Users
{
    public interface IUpdatePasswordService
    {
        Task<ResponseDto<bool>> Execute(UpdatePasswordDto updatePasswordDto);
    }
}
