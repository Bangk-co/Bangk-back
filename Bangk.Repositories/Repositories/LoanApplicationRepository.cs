using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class LoanApplicationRepository : GenericRepository<LoanApplication, LoanApplicationDto>, ILoanApplicationRepository
    {
        public LoanApplicationRepository(AppDbContext context) : base(context)
        {
        }

        public List<LoanApplicationDto> GetLoanApplicationInfo(int ClientId)
        {
            var query = from c in Context.Clients
                        join l in Context.LoanApplication on c.Id equals l.ClientId
                        join b in Context.Settings on l.BankId equals b.Id
                        join s in Context.Settings on l.StateId equals s.Id
                        join lt in Context.Settings on l.LoanTypeId equals lt.Id
                        join u in Context.Users on l.ReviewerId equals u.Id
                        where c.Id == ClientId
                        select new LoanApplicationDto
                        {
                            LoanTypeName = lt.Name,
                            LoanApplicationId = l.Id,
                            BankName = b.Name,
                            CreationDate = l.CreationDate,
                            LoanBalance = l.LoanBalance,
                            StateName = s.Name,
                            ReviewerName = string.Format("{0} {1}", u.Name, u.LastName),

                        };

            return query.ToList();

        }

        public async Task<LoanApplicationInfoDto> GetLoanApplicationById(int loanApplicationId)
        {
            var query = from c in Context.Clients
                        join l in Context.LoanApplication on c.Id equals l.ClientId
                        let observation = Context.LoanApplicationStateHistory.Where(h => h.StateId == (int)ApplicationStateHistoryEnum.RevisionDeExtracto &&
                            h.LoanApplicationId == loanApplicationId).FirstOrDefault()
                        let document = Context.LoanApplicationDocument.Where(d => d.LoanApplicationId == l.Id && d.DocumentType == (int)DocumentType.BankStatement).FirstOrDefault()
                        where l.Id == loanApplicationId
                        select new LoanApplicationInfoDto
                        {
                            BankId = l.BankId,
                            BirthDate = c.BirthDate,
                            ClientLastName = c.LastName,
                            ClientName = c.Name,
                            CountryCodePhoneId = c.CountryCodePhoneId,
                            Email = c.Email,
                            GenderId = c.GenderId,
                            IdentificationNumber = c.IdentificationNumber,
                            DocumenIdentificationTypeId = c.DocumenIdentificationTypeId,
                            LoanBalance = l.LoanBalance,
                            LoanNumber = l.LoanNumber,
                            LoanType = l.LoanTypeId,
                            PhoneNumber = c.PhoneNumber,
                            MonthlyCostInsurance = l.MonthlyCostInsurance,
                            Observations = observation == null ? string.Empty : observation.Observations,
                            ClientId = l.ClientId,
                            LoanApplicationId = l.Id,
                            BankStatementUrl = document == null ? string.Empty : document.Url,
                            BankStatementName = document == null ? string.Empty : document.Name,
                        };

            return await query.FirstOrDefaultAsync();

        }
    }
}
