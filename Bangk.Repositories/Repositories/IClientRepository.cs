using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface IClientRepository : IGenericRepository<Client, ClientDto>
    {
        Task<bool> ExistClientByEmailAndIdentificationNumber(string email, string identificationNumber);
        IQueryable<ClientDto> GetClientListQuery(string searchValue, string sortField, string sortOrder);

        IQueryable<ClientInfoDto> GetClientByIdInfo(int clientId);

        
    }
}
