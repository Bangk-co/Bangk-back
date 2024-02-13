using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.LoanQuote;
using Bangk.Repositories.Repositories;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class AcceptLoanQuoteService : IAcceptLoanQuoteService
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        private readonly IClientRepository _clientRepository;
        public AcceptLoanQuoteService(ILoanApplicationRepository loanApplicationRepository, ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository,
            IClientRepository clientRepository)
        {
            _loanApplicationRepository = loanApplicationRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _clientRepository = clientRepository;
        }

        public async Task<ResponseDto<bool>> Execute(int loanApplicationId)
        {
            var loanApplication = _loanApplicationRepository.GetById(loanApplicationId);
            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

            var result = new ResponseDto<bool>
            {
                Result = true
            };

            if (loanApplication == null)
            {
                result.HasErrors = true;
                result.Errors.Add($"La Applicacion ingresado no existe");
                result.Result = false;
                return result;
            }

            var loanApplicationHistory = new Repositories.Entities.LoanApplicationStateHistory
            {
                CreationDate = DateTime.Now,
                LoanApplicationId = loanApplicationId,
                Observations = string.Empty,
                StateId = (int)ApplicationStateHistoryEnum.CotizaciónAceptada
            };

            await _loanApplicationStateHistoryRepository.Create(loanApplicationHistory);
            var client = await _clientRepository.GetClientByLoanApplicationId(loanApplicationId);
            client.State = (int)ClientStateEnum.Prospecto;
            await _clientRepository.Update(client);
            scope.Complete();
            return result;
        }
    }
}
