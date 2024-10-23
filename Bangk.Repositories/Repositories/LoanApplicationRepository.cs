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

        public async Task<List<LoanApplicationDto>> GetLoanApplicationInfo(int ClientId, bool takeFirstThree = false)
        {
            var query = from c in Context.Clients
                        join l in Context.LoanApplication on c.Id equals l.ClientId
                        join b in Context.Settings on l.BankId equals b.Id
                        join lt in Context.Settings on l.LoanTypeId equals lt.Id
                        join u in Context.Users on l.ReviewerId equals u.Id
                        where c.Id == ClientId
                        select new LoanApplicationDto
                        {
                            LoanTypeName = lt.Name,
                            LoanApplicationId = l.Id,
                            BankName = b.Name,
                            IconsBanksUrl = b.IconsBanksUrl,
                            CreationDate = l.CreationDate,
                            LoanBalance = l.LoanBalance,
                            StateName = Context.LoanApplicationStateHistory.Where(m => m.LoanApplicationId == l.Id ).OrderByDescending(lh => lh.CreationDate).FirstOrDefault().State.Name,
                            ReviewerName = string.Format("{0} {1}", u.Name, u.LastName),

                        };

            if (takeFirstThree)
            {
                query = query.OrderByDescending(q => q.CreationDate).Take(3);
            }

            return await query.ToListAsync();
        }

        public async Task<LoanApplicationInfoDto> GetLoanApplicationById(int loanApplicationId)
        {
            var query = from c in Context.Clients
                        join l in Context.LoanApplication on c.Id equals l.ClientId
                        let observation = Context.LoanApplicationStateHistory.Where(h => h.StateId == (int)ApplicationStateHistoryEnum.RevisionDeExtracto &&
                            h.LoanApplicationId == loanApplicationId).FirstOrDefault()
                        let document = Context.LoanApplicationDocument.Where(d => d.LoanApplicationId == l.Id && d.DocumentType == (int)DocumentType.BankStatement).FirstOrDefault()
                        let documentIdentification = Context.LoanApplicationDocument.Where(d => d.LoanApplicationId == l.Id && d.DocumentType == (int)DocumentType.Identification).FirstOrDefault()
                        join s in Context.Settings on l.BankId equals s.Id
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
                            IdentificationUrl = document == null ? string.Empty : document.Url,
                            IdentificationNameUrl = document == null ? string.Empty : document.Name,
                            IconsBanksUrl = s == null ? string.Empty : s.IconsBanksUrl,
                            DocumentIssueDate = c.DocumentIssueDate
                        };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<LoanApplicationDto>> GetLoanApplicationInfoByLoanApplicationId(int loanApplicationId)
        {
            var query = from l in Context.LoanApplication
                        join b in Context.Settings on l.BankId equals b.Id
                        join lt in Context.Settings on l.LoanTypeId equals lt.Id
                        join u in Context.Users on l.ReviewerId equals u.Id
                        join lq in Context.LoanQuote on l.Id equals lq.LoanApplicationId into tlq
                        from sublq in tlq.DefaultIfEmpty()
                        where l.Id == loanApplicationId
                        select new LoanApplicationDto
                        {
                            LoanTypeName = lt.Name,
                            LoanApplicationId = l.Id,
                            BankName = b.Name,
                            IconsBanksUrl = b.IconsBanksUrl,
                            CreationDate = l.CreationDate,
                            LoanBalance = l.LoanBalance,
                            StateName = Context.LoanApplicationStateHistory.Where(m => m.LoanApplicationId == l.Id).OrderByDescending(lh => lh.CreationDate).FirstOrDefault().State.Name,
                            ReviewerName = string.Format("{0} {1}", u.Name, u.LastName),
                            YearlyFee = sublq.YearlyFee
                        };

            return await query.ToListAsync();
        }
    }
}
