using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            try
            {
                var query = from c in Context.Clients
                            let loans = Context.LoanApplication.Where(l => l.ClientId == c.Id).OrderBy(l => l.CreationDate).First()
                            where c.LoanApplications.Count > 0
                            orderby c.Id descending
                            select new ClientDto
                            {
                                Name = string.Format("{0} {1}", c.Name, c.LastName),
                                CreationDate = c.CreationDate,
                                LoanBalance = c.LoanApplications.Sum(l => l.LoanBalance),
                                ReviewerName = string.Format("{0} {1}", loans.Clients.Name, loans.Clients.LastName),
                                StateName = Context.Settings.FirstOrDefault(setting => setting.Id == loans.StateId).Name,
                                Applications = c.LoanApplications.Count()
                            };

                if (!string.IsNullOrEmpty(searchValue))
                {
                    query = query.Where(u => u.Name.Contains(searchValue));
                }

                return query.AsQueryable();

            }
            catch(Exception ex)
            {
                throw new Exception($"{ex}");
                    
            }

        }

    

        public Task<bool> ExistClientByEmailAndIdentificationNumber(string email, string identificationNumber)
        {
            return Context.Clients.AnyAsync(e => string.Equals(e.Email, email) &&  string.Equals(e.IdentificationNumber, identificationNumber));
        }
    }
}
