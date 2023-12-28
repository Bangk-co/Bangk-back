using Bangk.Domain.Contract.Users;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;

namespace Bangk.Domain.Implementation.Users
{
    public class GetUserByEmailService : IGetUserByEmailService
    {
        private readonly IUserRepository _userRepository;
        public GetUserByEmailService(IUserRepository repository)
        {
            _userRepository = repository;
        }

        public User Execute(string email)
        {
            return _userRepository.GetUserByEmail(email);
        }
    }
}
