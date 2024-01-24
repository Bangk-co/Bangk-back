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
            var query = from c in Context.Clients
                        let loans = Context.LoanApplication.Where(l => l.ClientId == c.Id).OrderBy(l => l.CreationDate).First()
                        where c.LoanApplications.Count > 0
                        orderby c.Id descending
                        select new ClientDto
                        {
                            Name = string.Format("{0} {1}", c.Name, c.LastName),
                            CreationDate = c.CreationDate,
                            LoanBalance = c.LoanApplications.Sum(l => l.LoanBalance),
                            ReviewerName = string.Format("{0} {1}", loans.Client.Name, loans.Client.LastName),
                            StateName = Context.Settings.FirstOrDefault(setting => setting.Id == loans.StateId).Name,
                            Applications = c.LoanApplications.Count()
                        };

            if (!string.IsNullOrEmpty(searchValue))
            {
                if (!string.IsNullOrEmpty(searchValue))
                {
                    query = query.Where(u => u.Name.Contains(searchValue));
                }

                return query.AsQueryable();

            }

            return query.AsQueryable();
        }

        public IQueryable<ClientInfoDto> GetClientByIdInfo(int clientId)
        {
            var query = from c in Context.Clients
                        join s in Context.Settings on c.DocumenIdentificationTypeId equals s.Id
                        join sg in Context.Settings on c.GenderId equals sg.Id
                        join sc in Context.Settings on c.CountryCodePhoneId equals sc.Id
                        where c.Id == clientId
                        select new ClientInfoDto
                        {

                            Name = string.Format("{0} {1}", c.Name, c.LastName),
                            DocumentType = s.Name,
                            IdentificationNumber = c.IdentificationNumber,
                            IdClient = c.Id,
                            Email = c.Email,
                            CountryCode = sc.Id,
                            PhoneNumber = c.PhoneNumber,
                            BirthDate = c.BirthDate,
                            GenderName = sg.Name


                        };

            return query;
        }

        public Task<bool> ExistClientByEmailAndIdentificationNumber(string email, string identificationNumber)
        {
            return Context.Clients.AnyAsync(e => string.Equals(e.Email, email) && string.Equals(e.IdentificationNumber, identificationNumber));
        }
    }
}
