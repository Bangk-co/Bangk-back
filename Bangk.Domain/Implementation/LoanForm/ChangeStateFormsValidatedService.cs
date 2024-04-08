using Bangk.Common.Dto;
using Bangk.Domain.Contract.Integrations;
using Bangk.Domain.Contract.LoanForm;
using Bangk.Domain.Implementation.Integrations;
using Bangk.Repositories.Repositories;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Bangk.Domain.Implementation.LoanForm
{
    public class ChangeStateFormsValidatedService : IChangeStateFormsValidatedService
    {
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IValidationSBSService _validationSBSService;
        public ChangeStateFormsValidatedService(ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository,
            ILoanApplicationRepository loanApplicationRepository, IValidationSBSService validationSBSService)
        {
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _loanApplicationRepository = loanApplicationRepository;
            _validationSBSService = validationSBSService;
        }

        public async Task<ResponseDto<bool>> Execute(int loanApplicationId, string observation)
        {
            var loanApplication = _loanApplicationRepository.GetById(loanApplicationId);
            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

            var result = new ResponseDto<bool>
            {
                Result = true
            };
            /* 
            if (loanApplication == null)
            {
                result.HasErrors = true;
                result.Errors.Add($"La Applicacion ingresado no existe");
                result.Result = false;
                return result;
            }

            await SaveLoanHistory(loanApplicationId, observation, (int)ApplicationStateHistoryEnum.FormulariosValidados);
           
           */

            await _validationSBSService.Execute(loanApplicationId);
            scope.Complete();
            return result;

        }

        private async Task SaveLoanHistory(int loanApplicationId, string observation, int state)
        {
            var loanApplicationHistory = new Repositories.Entities.LoanApplicationStateHistory
            {
                CreationDate = DateTime.Now,
                LoanApplicationId = loanApplicationId,
                Observations = observation ?? string.Empty,
                StateId = state
            };

            await _loanApplicationStateHistoryRepository.Create(loanApplicationHistory);
        }

      
    }
}
