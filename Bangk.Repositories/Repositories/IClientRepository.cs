using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;

namespace Bangk.Repositories.Repositories
{
    public interface IClientRepository : IGenericRepository<Client, ClientDto>
    {
    }
}
