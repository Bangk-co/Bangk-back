using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.SqlServer.Server;
using System.Linq;

namespace Bangk.Repositories.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public User GetUserByEmail(string email)
        {
            var user = Context.Users.FirstOrDefault(e => e.Email == email);
            if(user != null)
            {
                user.Role = Context.Roles.FirstOrDefault(r => r.Id == user.RoleId);
            }

            return user;
        }

        public User GetUserByHashEmailReset(string hash)
        {
            var user = Context.Users.FirstOrDefault(e => e.PasswordResetGuid == hash);
            return user;
        }
    }
}
