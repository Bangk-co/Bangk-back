using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class UserRepository : GenericRepository<User, UserDto>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public User GetUserByEmail(string email)
        {
            var user = Context.Users.FirstOrDefault(e => e.Email == email);
            if (user != null)
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

        public async Task<User> GetUserWithLessApplication()
        {
            var query = from user in Context.Users
                        where user.RoleId == 2
                        orderby user.LoanApplications.Count()
                        select user;
            return await query.FirstOrDefaultAsync();
        }

        public User GetByClientId(int clientId)
        {
            return Context.Users.FirstOrDefault(u => u.ClientId == clientId);
        }

        public async Task<UserInfoDto> GetUserInfoByClientId(int clientId)
        {
            var query = from cl in Context.Clients
                        where cl.Id == clientId
                        select new UserInfoDto
                        {
                            IdentificationNumber = cl.IdentificationNumber,
                            DocumenIdentificationTypeId = cl.DocumenIdentificationTypeId,
                            Email = cl.Email,
                            LastName = cl.LastName,
                            Name = cl.Name
                        };

            return await query.FirstOrDefaultAsync();
        }
    }
}
