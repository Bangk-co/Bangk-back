using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Login
{
    public interface IUserLoginService
    {
        Task<ResponseDto<string>> Execute(LoginDto login);
    }
}
