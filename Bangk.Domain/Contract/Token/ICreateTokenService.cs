using Bangk.Repositories.Entities;

namespace Bangk.Domain.Contract.Login
{
    public interface ICreateTokenService 
    {
        string Execute(User user);
    }
}
