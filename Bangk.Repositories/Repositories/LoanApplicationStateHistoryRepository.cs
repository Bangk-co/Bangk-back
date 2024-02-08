using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class LoanApplicationStateHistoryRepository : GenericRepository<LoanApplicationStateHistory, LoanApplicationStateHistoryDto>, ILoanApplicationStateHistoryRepository
    {
        public LoanApplicationStateHistoryRepository(AppDbContext context) : base(context)
        {
        }

        public IQueryable<LoanApplicationStateHistoryDto> GetLoanApplicationStateHistory(int loanApplicationId)
        {

            var enumType = typeof(ApplicationStateHistoryEnum);

            var query = from history in Context.LoanApplicationStateHistory
                        where history.LoanApplicationId == loanApplicationId
                        orderby history.StateId descending
                        select new LoanApplicationStateHistoryDto
                        {
                            StateName = Enum.GetName(enumType, history.StateId),
                            CreationDate = history.CreationDate,
                            Observations = history.Observations
                        };

            return query.AsQueryable();
        }

        public async Task<LoanApplicationStateHistory> GetLoanApplicationStateHistoryByStateAndLoanApplicationId(int stateId, int loanApplicationId)
        {
           return await Context.LoanApplicationStateHistory.Where(h => h.StateId == stateId && h.LoanApplicationId == loanApplicationId).FirstOrDefaultAsync();
        }
    }
}
