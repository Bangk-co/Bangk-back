using Bangk.Common.Dto;
using Bangk.Domain.Contract.Users;
using Bangk.Repositories.Repositories;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Users
{
    public class GetUserInfoByClientIdService : IGetUserInfoByClientIdService
    {
        private readonly IUserRepository _userRepository;
        public GetUserInfoByClientIdService(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }

        public Task<UserInfoDto> Execute(int clientId)
        {
            return _userRepository.GetUserInfoByClientId(clientId);
        }
    }
}
