using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
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



            var subquery = from l in Context.LoanApplication
                           group l by l.ClientId into clientGroup
                           select new
                           {
                               ClientId = clientGroup.Key,
                               LoanApplicationsCount = clientGroup.Count()
                           };

            var query = from l in Context.LoanApplication
                        join c in Context.Clients on l.ClientId equals c.Id
                        join u in Context.Users on l.ReviewerId equals u.Id
                        join s in Context.Settings on l.StateId equals s.Id
                        join sub in subquery on c.Id equals sub.ClientId into subGroup
                        from sub in subGroup.DefaultIfEmpty()
                        orderby l.Id descending
                        select new ClientDto
                        {
                            Name = string.Format("{0} {1}", c.Name, c.LastName),
                            CreationDate = l.CreationDate,
                            LoanBalance = l.LoanBalance,
                            ReviewerName = string.Format("{0} {1}", u.Name, u.LastName),
                            StateName = s.Name,
                            Applications = sub.LoanApplicationsCount
                        };

            



            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(u => u.Name.Contains(searchValue));
            }

            return query.AsQueryable();
        }

        public Task<Client> GetClientes (int id)
        {
            var client = GetById(id);
            return client;
        } 


        public Task<bool> ExistClientByEmailAndIdentificationNumber(string email, string identificationNumber)
        {
            return Context.Clients.AnyAsync(e => string.Equals(e.Email, email) &&  string.Equals(e.IdentificationNumber, identificationNumber));
        }
    }
}
