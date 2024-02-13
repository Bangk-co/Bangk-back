using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class LoanQuoteRepository : GenericRepository<LoanQuote, LoanQuoteDto>, ILoanQuoteRepository
    {
        public LoanQuoteRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<GetLoanQuoteDto> GetLoanQuoteInfoByLoanApplicationId(int loanApplicationId)
        {
            var query = from l in Context.LoanApplication
                        join c in Context.Clients on l.ClientId equals c.Id
                        join type in Context.Settings on l.LoanTypeId equals type.Id
                        join lq in Context.LoanQuote on l.Id equals lq.LoanApplicationId
                        join insu in Context.Settings on lq.InsuranceId equals insu.Id
                        where l.Id == loanApplicationId
                        select new GetLoanQuoteDto
                        {
                            LoanApplicationId = l.Id,
                            ClientLastName = c.LastName,
                            ClientName = c.Name,
                            InsuraceCurrierName = insu.Name,
                            LoanBalance = l.LoanBalance,
                            LoanTypeName = type.Name,
                            YearlyFee = lq.YearlyFee,
                            MonthlyCostInsurance = l.MonthlyCostInsurance
                        };
            return await query.FirstOrDefaultAsync();
        }
    }
}
