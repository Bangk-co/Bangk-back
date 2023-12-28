using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;

namespace Bangk.Repositories.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User GetUserByEmail(string email);
        User GetUserByHashEmailReset(string hash);
    }
}
