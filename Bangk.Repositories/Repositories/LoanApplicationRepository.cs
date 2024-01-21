using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var query = from c in Context.Clients join
                        l in Context.LoanApplication on c.Id equals l.ClientId join
                        b in Context.Settings on l.BankId equals b.Id join
                        s in Context.Settings on l.StateId equals s.Id join
                        lt in Context.Settings on l.LoanType equals lt.Id join
                        u in Context.Users on l.ReviewerId equals u.Id
                        where c.Id == ClientId
                        select new LoanApplicationDto
                        {
                            ClientName = string.Format("{0} {1}", c.Name, c.LastName),
                            LoanBalance = l.LoanBalance,
                            BankName = b.Name,
                            LoanTypeName = lt.Name,
                            LoanNumber = l.LoanNumber,
                            ReviewerName = string.Format("{0} {1}", u.Name, u.LastName),
                            StateName = s.Name
                        };

            return query.ToList();

        }
    }
}
