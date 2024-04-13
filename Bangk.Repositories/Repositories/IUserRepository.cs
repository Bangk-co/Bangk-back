using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface IUserRepository : IGenericRepository<User, UserDto>
    {
        User GetUserByEmail(string email);
        User GetUserByHashEmailReset(string hash);
        Task<User> GetUserWithLessApplication();
    }
}
