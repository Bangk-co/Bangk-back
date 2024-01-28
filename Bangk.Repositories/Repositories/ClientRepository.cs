using Bangk.Common.Dto;
using Bangk.Common.Utils;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class ClientRepository : GenericRepository<Client, ClientDto>, IClientRepository
    {
        public ClientRepository(AppDbContext context) : base(context)
        {
        }


        public IQueryable<ClientDto> GetClientListQuery(string searchValue, string sortField, string sortOrder)
        {
            var query = from c in Context.Clients
                        let loans = Context.LoanApplication.Where(l => l.ClientId == c.Id).OrderBy(l => l.CreationDate).FirstOrDefault()
                        where c.LoanApplications.Count > 0 &&
                              (string.IsNullOrEmpty(searchValue) || c.Name.Contains(searchValue))
                        select new ClientDto
                        {
                            IdClient = c.Id,
                            Name = c.Name + " " + c.LastName,  
                            CreationDate = c.CreationDate,
                            LoanBalance = c.LoanApplications.Sum(l => l.LoanBalance),
                            ReviewerName = loans != null ? Context.Users.FirstOrDefault(u => u.Id == loans.ReviewerId).Name + " " + Context.Users.FirstOrDefault(u => u.Id == loans.ReviewerId).LastName : string.Empty,
                            StateName = loans != null ? Context.Settings.FirstOrDefault(setting => setting.Id == loans.StateId).Name : string.Empty,
                            Applications = c.LoanApplications.Count()
                        };

            if (!string.IsNullOrEmpty(sortField) && !string.IsNullOrEmpty(sortOrder))
            {
                query = ExtensionMethods.OrderBy(query, sortField, sortOrder.ToLower() == "desc");
            }

            return query.AsQueryable();
        }

        public IQueryable<ClientInfoDto> GetClientByIdInfo(int clientId)
        {
            var query = from c in Context.Clients
                        let loans = Context.LoanApplication.Where(l => l.ClientId == c.Id).OrderBy(l => l.CreationDate).FirstOrDefault()
                        join s in Context.Settings on c.DocumenIdentificationTypeId equals s.Id
                        join sg in Context.Settings on c.GenderId equals sg.Id
                        join sc in Context.Settings on c.CountryCodePhoneId equals sc.Id
                        where c.Id == clientId
                        select new ClientInfoDto
                        {

                            Name = c.Name,
                            LastName = c.LastName,
                            DocumentType = s.Name,
                            IdentificationNumber = c.IdentificationNumber,
                            IdClient = c.Id,
                            Email = c.Email,
                            CountryCode = sc.Id,
                            PhoneNumber = c.PhoneNumber,
                            BirthDate = c.BirthDate,
                            GenderName = sg.Name,
                            StateName = loans != null ? Context.Settings.FirstOrDefault(setting => setting.Id == loans.StateId).Name : string.Empty

                        };

            return query;
        }

        public async Task<bool> ExistClientByEmailAndIdentificationNumber(string email, string identificationNumber)
        {
            return await Context.Clients.AnyAsync(e => string.Equals(e.Email, email) && string.Equals(e.IdentificationNumber, identificationNumber));
        }



    }
}
