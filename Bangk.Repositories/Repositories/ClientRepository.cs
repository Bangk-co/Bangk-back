using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class ClientRepository : GenericRepository<Client, ClientDto>, IClientRepository
    {
        public ClientRepository(AppDbContext context) : base(context)
        {
        }

        

        public IQueryable<ClientDto> GetClientListQuery(string searchValue)
        {

           

            var query = from c in Context.Clients
                        orderby c.Id descending
                        select new ClientDto
                        {
                            
                            Name = c.Name,
                            LastName = c.LastName,
                            PhoneNumber = c.PhoneNumber,
                            Email = c.Email,
                            IdentificationNumber = c.IdentificationNumber,
                            BirthDate = c.BirthDate,
                            GenderId = c.GenderId,
                            IdentificationFileUrl = c.IdentificationFileUrl,
                            DocumenIdentificationTypeId = c.DocumenIdentificationTypeId

                        };



            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(u => u.Name.Contains(searchValue));
            }

            return query;
        }


        public Task<bool> ExistClientByEmailAndIdentificationNumber(string email, string identificationNumber)
        {
            return Context.Clients.AnyAsync(e => string.Equals(e.Email, email) &&  string.Equals(e.IdentificationNumber, identificationNumber));
        }
    }
}
