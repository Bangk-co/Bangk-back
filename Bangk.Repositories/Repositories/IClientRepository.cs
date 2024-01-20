using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface IClientRepository : IGenericRepository<Client, ClientDto>
    {
        Task<bool> ExistClientByEmailAndIdentificationNumber(string email, string identificationNumber);
    }
}
