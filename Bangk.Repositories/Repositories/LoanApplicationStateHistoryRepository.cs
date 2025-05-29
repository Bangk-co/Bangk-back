using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
                        orderby history.CreationDate descending
                        select new LoanApplicationStateHistoryDto
                        {
                            Id = history.Id,
                            StateName = Enum.GetName(enumType, history.StateId),
                            CreationDate = history.CreationDate,
                            Observations = history.Observations,
                            ValidationClientResult = history.ValidationClientResult
                        };

            return query.AsQueryable();
        }

        public async Task<LoanApplicationStateHistory> GetLoanApplicationStateHistoryByStateAndLoanApplicationId(int stateId, int loanApplicationId)
        {
            return await Context.LoanApplicationStateHistory.Where(h => h.StateId == stateId && h.LoanApplicationId == loanApplicationId).FirstOrDefaultAsync();
        }

        public async Task<List<LoanApplicationStateDto>> GetLoanApplicationQuoteAccepted()
        {
            var query = from application in Context.LoanApplication
                        join client in Context.Clients on application.ClientId equals client.Id
                        where Context.LoanApplicationStateHistory
                            .Where(history => history.LoanApplicationId == application.Id)
                            .OrderByDescending(history => history.CreationDate)
                            .FirstOrDefault().StateId == (int)ApplicationStateHistoryEnum.CotizaciónAceptada
                        select new LoanApplicationStateDto
                        {
                            ClientEmail = client.Email,
                            clientId = client.Id,
                            ClientLastName = client.LastName,
                            ClientName = client.Name,
                            LoanApplicationId = application.Id,
                            CreationDate = Context.LoanApplicationStateHistory
                                .Where(history => history.LoanApplicationId == application.Id)
                                .OrderByDescending(history => history.CreationDate)
                                .FirstOrDefault().CreationDate
                        };

            return await query.ToListAsync();
        }

        public async Task<LoanApplicationStateHistory> GetLastStateByLoanApplicationId(int loanApplicationId)
        {
            return await Context.LoanApplicationStateHistory.Where(h => h.LoanApplicationId == loanApplicationId)
                .OrderByDescending(h => h.CreationDate).FirstOrDefaultAsync();
        }
    }
}
