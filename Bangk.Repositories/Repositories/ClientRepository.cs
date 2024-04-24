using Bangk.Common.Dto;
using Bangk.Common.Enums;
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
                            DocumentIssueDate = c.DocumentIssueDate,
                            GenderName = sg.Name,
                            StateName = loans != null ? Context.Settings.FirstOrDefault(setting => setting.Id == c.State).Name : string.Empty
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

        public  IQueryable<ClientReportDto> GetReportClientsSarlaft (ApplicationStateHistoryEnum stateloanApplication)
        {
            var query = from c in Context.Clients
                        join l in Context.LoanApplication on c.Id equals l.ClientId
                        join lh in Context.LoanApplicationStateHistory on l.Id equals lh.LoanApplicationId
                        join lp in Context.LoanInsurancePolicy on l.Id equals lp.LoanApplicationId into t
                        from sublp in t.DefaultIfEmpty()
                        join st in Context.Settings on l.BankId equals st.Id
                        join ci in Context.Cities on sublp.BirthCityId equals ci.Id into ciGroup
                        from ci in ciGroup.DefaultIfEmpty()
                        join stn in Context.State on ci.StateId equals stn.Id into stnGroup
                        from stn in stnGroup.DefaultIfEmpty()
                        join ct in Context.Countries on stn.CountryId equals ct.Id into ctGroup
                        from ct in ctGroup.DefaultIfEmpty()
                        join lq in Context.LoanQuote on l.Id equals lq.LoanApplicationId into lqGroup
                        from lq in lqGroup.DefaultIfEmpty()
                        join bn in Context.Beneficiaries on sublp.Id equals bn.LoanInsurancePolicyId into bnGroup
                        from bn in bnGroup.DefaultIfEmpty()
                        where lh.StateId == (int)stateloanApplication
                        select new ClientReportDto
                        {

                            DocumenIdentificationTypeId = c.DocumenIdentificationTypeId != 0 ? c.DocumenIdentificationTypeId : 0,
                            IdentificationNumber = c.IdentificationNumber != null ? c.IdentificationNumber : string.Empty,
                            Name = c.Name != null ? c.Name : string.Empty,
                            LastName = c.LastName != null ? c.LastName : string.Empty,
                            Address = sublp.Address != null ? sublp.Address : string.Empty,
                            CityName = ci.Name ?? string.Empty,
                            StateName = stn.Name ?? string.Empty,
                            CountryName = ct.Name ?? string.Empty,
                            PhoneNumber = c.PhoneNumber != null ? c.PhoneNumber : string.Empty,
                            Email = c.Email != null ? c.Email : string.Empty,
                            BirthDate = c.BirthDate != DateTime.Now ? c.BirthDate : DateTime.Now,
                            CreationDatePolicy = sublp.CreationDate != DateTime.Now ? sublp.CreationDate : DateTime.Now,
                            NameBank = st.Name != null ? st.Name : string.Empty,
                            NitBank = st.Code != null ? st.Code : string.Empty,
                            LoanBalance = l.LoanBalance != 0 ? l.LoanBalance : 0,
                            YearlyFee = lq.YearlyFee != 0 ? lq.YearlyFee : 0,
                            Beneficiary = (
                            from b in Context.Beneficiaries 
                            where b.LoanInsurancePolicyId == sublp.Id
                            select new BeneficiariesJobDto
                            {
                                Name = b.Name,
                                LastName = b.LastName,
                                RelationshipId = b.RelationshipId,
                                PercentInherit = b.PercentInherit

                            }).ToList(),

                        };


            return query;


        }
    }
}
