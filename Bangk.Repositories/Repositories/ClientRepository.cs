using Bangk.Common.Dto;
using Bangk.Common.Utils;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class ClientRepository : GenericRepository<Client, ClientDto>, IClientRepository
    {
        public ClientRepository(AppDbContext context) : base(context)
        {
        }


        public IQueryable<ClientDto> GetClientListQuery(string searchValue, string sortField, string sortOrder, ClaimsPrincipal user)
        {
            var query = from c in Context.Clients
                        join state in Context.Settings on c.State equals state.Id
                        let loans = Context.LoanApplication.Where(l => l.ClientId == c.Id).OrderBy(l => l.CreationDate).FirstOrDefault()
                        where c.LoanApplications.Count > 0 &&
                              (string.IsNullOrEmpty(searchValue) || c.Name.Contains(searchValue) || c.LastName.Contains(searchValue) ||
                              (c.Name + " " + c.LastName).Contains(searchValue) || c.IdentificationNumber.Contains(searchValue) ||
                              c.Email.Contains(searchValue))
                        select new ClientDto
                        {
                            IdClient = c.Id,
                            Name = c.Name + " " + c.LastName,
                            CreationDate = c.CreationDate,
                            LoanBalance = c.LoanApplications.Sum(l => l.LoanBalance),
                            ReviewerName = loans != null ? Context.Users.FirstOrDefault(u => u.Id == loans.ReviewerId).Name + " " + Context.Users.FirstOrDefault(u => u.Id == loans.ReviewerId).LastName : string.Empty,
                            StateName = state.Name,
                            Applications = c.LoanApplications.Count(),
                            ReviewerId = loans.ReviewerId
                        };
            if (user.IsInRole("asesor"))
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                query = query.Where(c => c.ReviewerId.ToString() == userId);
            }

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

                        join sg in Context.Settings on c.GenderId equals sg.Id into t
                        from subsg in t.DefaultIfEmpty()

                        join sc in Context.Settings on c.CountryCodePhoneId equals sc.Id into r
                        from subsc in r.DefaultIfEmpty()

                        where c.Id == clientId
                        select new ClientInfoDto
                        {
                            Name = c.Name,
                            LastName = c.LastName,
                            DocumentType = s.Name,
                            DocumentTypeId = s.Id,
                            IdentificationNumber = c.IdentificationNumber,
                            IdClient = c.Id,
                            Email = c.Email,
                            CountryCode = subsc.Id,
                            PhoneNumber = c.PhoneNumber,
                            BirthDate = c.BirthDate,
                            DocumentIssueDate = c.DocumentIssueDate,
                            GenderName = subsg.Name,
                            GenderId = subsg.Id,
                            StateName = loans != null ? Context.Settings.FirstOrDefault(setting => setting.Id == c.State).Name : string.Empty,
                            ClientAge = c.BirthDate != null ? ((DateTime.Now.Year - c.BirthDate.Value.Year) >= 65 && (DateTime.Now.Year - c.BirthDate.Value.Year) <= 74) : false
                        };

            return query;
        }

        public async Task<bool> ExistClientByEmailAndIdentificationNumber(string email, string identificationNumber)
        {
            return await Context.Clients.AnyAsync(e => string.Equals(e.Email, email) || string.Equals(e.IdentificationNumber, identificationNumber));
        }

        public async Task<Client> GetClientByLoanApplicationId(int loanApplicationId)
        {
            var query = from c in Context.Clients
                        join l in Context.LoanApplication on c.Id equals l.ClientId
                        where l.Id == loanApplicationId
                        select c;

            return await query.FirstOrDefaultAsync();
        }

        public IQueryable<ClientPolicyDto> GetPolicyClient(int clientId)
        {
            var query = from lp in Context.LoanInsurancePolicy
                        join l in Context.LoanApplication on lp.LoanApplicationId equals l.Id
                        join c in Context.Clients on l.ClientId equals c.Id
                        join s in Context.Settings on l.LoanTypeId equals s.Id into settingsGroup
                        from sj in settingsGroup.DefaultIfEmpty()
                        where c.Id == clientId
                        select new
                        {
                            LoanApplicationId = l.Id,
                            PolicyId = lp.Id,
                            LoanType = sj != null ? sj.Name : null,
                            CreationDateLoanApplication = l.CreationDate,
                            CreationDatePolicy = lp.CreationDate,
                            StateName = Context.LoanApplicationStateHistory
                                         .Where(m => m.LoanApplicationId == l.Id)
                                         .OrderByDescending(lh => lh.CreationDate)
                                         .Select(lh => lh.State.Name)
                                         .FirstOrDefault()
                        };

            var result = query.ToList().Select(item => new ClientPolicyDto
            {
                LoanApplicationId = item.LoanApplicationId,
                PolicyId = item.PolicyId,
                LoanType = item.LoanType,
                CreationDateLoanApplication = item.CreationDateLoanApplication,
                CreationDatePolicy = item.CreationDatePolicy,
                StateName = item.StateName
            }).ToList();

            if (!result.Any() && Context.LoanApplication.Any(l => l.ClientId == clientId))
            {
                var fallbackQuery = from l in Context.LoanApplication
                                    join s in Context.Settings on l.LoanTypeId equals s.Id into settingsGroup
                                    from sj in settingsGroup.DefaultIfEmpty()
                                    where l.ClientId == clientId
                                    select new ClientPolicyDto
                                    {
                                        LoanApplicationId = l.Id,
                                        PolicyId = null,
                                        LoanType = sj != null ? sj.Name : null,
                                        CreationDateLoanApplication = l.CreationDate,
                                        CreationDatePolicy = null,
                                        StateName = Context.LoanApplicationStateHistory
                                         .Where(m => m.LoanApplicationId == l.Id)
                                         .OrderByDescending(lh => lh.CreationDate)
                                         .Select(lh => lh.State.Name)
                                         .FirstOrDefault()
                                    };

                result.AddRange(fallbackQuery);
            }

            return result.AsQueryable();
        }




    }
}
