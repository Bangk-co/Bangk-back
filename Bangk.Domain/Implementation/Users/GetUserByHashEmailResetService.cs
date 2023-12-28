using Bangk.Domain.Contract.Users;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;

namespace Bangk.Domain.Implementation.Users
{
    public class GetUserByHashEmailResetService : IGetUserByHashEmailResetService
    {

        private readonly IUserRepository _userRepository;
        public GetUserByHashEmailResetService(IUserRepository repository)
        {
            _userRepository = repository;
        }

        public User Execute(string hash)
        {
            return _userRepository.GetUserByHashEmailReset(hash);
        }
    }
}
