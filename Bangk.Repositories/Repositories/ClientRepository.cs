using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
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


        public IQueryable<ClientDto> GetClientListQuery(string searchValue)
        {
            var query = from c in Context.Clients
                        let loans = Context.LoanApplication.Where(l => l.ClientId == c.Id).OrderBy(l => l.CreationDate).FirstOrDefault()
                        where c.LoanApplications.Count > 0 &&
                              (string.IsNullOrEmpty(searchValue) || c.Name.Contains(searchValue))
                        orderby c.Id descending
                        select new ClientDto
                        {
                            IdClient = c.Id,
                            Name = string.Format("{0} {1}", c.Name, c.LastName),
                            CreationDate = c.CreationDate,
                            LoanBalance = c.LoanApplications.Sum(l => l.LoanBalance),
                            ReviewerName = loans != null ? string.Format("{0} {1}", loans.Client.Name, loans.Client.LastName) : string.Empty,
                            StateName = loans != null ? Context.Settings.FirstOrDefault(setting => setting.Id == loans.StateId).Name : string.Empty,
                            Applications = c.LoanApplications.Count()
                        };

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

        public async Task<ClientUpdateInfoDto> ClientByIdUpdate(int clienteId, ClientUpdateInfoDto updateClient)
        {
            var client = GetById(clienteId).Result;

            if (client != null)
            {

                client.Name = !string.IsNullOrEmpty(updateClient.Name) ? updateClient.Name : client.Name;
                client.LastName = !string.IsNullOrEmpty(updateClient.LastName) ? updateClient.LastName : client.LastName;
                client.DocumenIdentificationTypeId = updateClient.DocumentTypeId != 0 ? updateClient.DocumentTypeId : client.DocumenIdentificationTypeId;
                client.IdentificationNumber = !string.IsNullOrEmpty(updateClient.IdentificationNumber) ? updateClient.IdentificationNumber : client.IdentificationNumber;
                client.Email = !string.IsNullOrEmpty(updateClient.Email) ? updateClient.Email : client.Email;
                client.CountryCodePhoneId = updateClient.CountryCode != 0 ? updateClient.CountryCode : client.CountryCodePhoneId;
                client.PhoneNumber = !string.IsNullOrEmpty(updateClient.PhoneNumber) ? updateClient.PhoneNumber : client.PhoneNumber;
                client.BirthDate = updateClient.BirthDate != new DateTime() ? updateClient.BirthDate : client.BirthDate;
                client.GenderId = updateClient.GenderId != 0 ? updateClient.GenderId : client.GenderId;

                bool result = Update(client).Result;
                if (!result) return null;

                return updateClient;
            }

            return null;
        }


    }
}
