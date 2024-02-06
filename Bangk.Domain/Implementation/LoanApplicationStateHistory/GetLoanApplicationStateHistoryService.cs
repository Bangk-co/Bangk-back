using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplicationStateHistory;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanApplicationStateHistory
{
    public class GetLoanApplicationStateHistoryService : ILoanApplicationStateHistory
    {
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;

        public GetLoanApplicationStateHistoryService(ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository)
        {
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
        }

        public async Task<IQueryable<LoanApplicationStateHistoryDto>> Execute(int loanApplicationId)
        {
            var query =  _loanApplicationStateHistoryRepository.GetLoanApplicationStateHistory(loanApplicationId);
            return query;

        }
    }
}
