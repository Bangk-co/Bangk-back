using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.LoanQuote;
using Bangk.Repositories.Repositories;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class RejectLoanQuoteService : IRejectLoanQuoteService
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        private readonly IClientRepository _clientRepository;
        public RejectLoanQuoteService(ILoanApplicationRepository loanApplicationRepository, ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository,
            IClientRepository clientRepository)
        {
            _loanApplicationRepository = loanApplicationRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _clientRepository = clientRepository;
        }

        public async Task<ResponseDto<bool>> Execute(int loanApplicationId, string observation)
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
                CreationDate = DateTime.UtcNow,
                LoanApplicationId = loanApplicationId,
                Observations = observation,
                StateId = (int)ApplicationStateHistoryEnum.CotizaciónRechazada
            };

            await _loanApplicationStateHistoryRepository.Create(loanApplicationHistory);
            var client = await _clientRepository.GetClientByLoanApplicationId(loanApplicationId);
            client.State = (int)ClientStateEnum.PerdidoRechazo;
            await _clientRepository.Update(client);
            scope.Complete();
            return result;
        }
    }
}
