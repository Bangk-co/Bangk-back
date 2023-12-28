using Bangk.Repositories.Entities;

namespace Bangk.Domain.Contract.Users
{
    public interface IGetUserByEmailService
    {
        User Execute(string email);
    }
}
