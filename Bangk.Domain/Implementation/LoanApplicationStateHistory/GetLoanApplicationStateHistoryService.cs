using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.LoanApplicationStateHistory;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
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

        public async Task<LoanApplicationStateHistoryResultDto> Execute(int loanApplicationId)
        {
            var query =  _loanApplicationStateHistoryRepository.GetLoanApplicationStateHistory(loanApplicationId);
            var response = new LoanApplicationStateHistoryResultDto();
            List<string> nameStates = new List<string>();
            foreach (var state in Enum.GetValues(typeof(ApplicationStateHistoryEnum)))
            {
                nameStates.Add(state.ToString());
            }

            response.States = new List<string>();
            response.States.AddRange(nameStates);
            response.LoanApplicationHistory = query;
            return response;

        }
    }
}
