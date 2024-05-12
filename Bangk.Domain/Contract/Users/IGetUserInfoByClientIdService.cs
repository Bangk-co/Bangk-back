using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Users
{
    public interface IGetUserInfoByClientIdService
    {
        Task<UserInfoDto> Execute(int clientId);
    }
}
