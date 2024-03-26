using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.LoanApplication;
using Bangk.Repositories.Repositories;
using FluentValidation;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class UpdateLoanApplicationService : IUpdateLoanApplicationService
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IValidator<UpdateLoanApplicationDto> _updateLoanApplicationValidator;
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;

        public UpdateLoanApplicationService(ILoanApplicationRepository LoanApplicationRepository, IValidator<UpdateLoanApplicationDto> updateLoanApplicationValidator, ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository)
        {
            _updateLoanApplicationValidator = updateLoanApplicationValidator;
            _loanApplicationRepository = LoanApplicationRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
        }

        public async Task<ResponseDto<bool>> Execute(UpdateLoanApplicationDto updateLoanApplicationDto)
        {
            var result = new ResponseDto<bool>
            {
                Result = true
            };

            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);
            var validationResult = await _updateLoanApplicationValidator.ValidateAsync(updateLoanApplicationDto);
            if (!validationResult.IsValid)
            {
                result.HasErrors = true;
                result.Result = false;
                foreach (var error in validationResult.Errors)
                    result.Errors.Add(error.ErrorMessage);
                return result;
            }

            var loanApplication = await _loanApplicationRepository.GetById(updateLoanApplicationDto.LoanApplicationId);
            if (loanApplication == null)
            {
                result.HasErrors = true;
                result.Errors.Add($"La applicacion ingresado no existe");
                result.Result = false;
                return result;
            }

            loanApplication.LoanNumber = updateLoanApplicationDto.LoanNumber;
            loanApplication.BankId = updateLoanApplicationDto.BankId;
            loanApplication.LoanBalance = updateLoanApplicationDto.LoanBalance;
            loanApplication.MonthlyCostInsurance = updateLoanApplicationDto.MonthlyCostInsurance;
            loanApplication.LoanTypeId = updateLoanApplicationDto.LoanTypeId;
            await _loanApplicationRepository.Update(loanApplication);

            if (updateLoanApplicationDto.IsValidateLoanApplication)
            {
                var loanApplicationHistory = await _loanApplicationStateHistoryRepository.GetLoanApplicationStateHistoryByStateAndLoanApplicationId((int)ApplicationStateHistoryEnum.RevisionDeExtracto,
                    updateLoanApplicationDto.LoanApplicationId);
                if (loanApplicationHistory != null)
                {
                    loanApplicationHistory.CreationDate = DateTime.UtcNow;
                    loanApplicationHistory.Observations = updateLoanApplicationDto.Observations;
                    await _loanApplicationStateHistoryRepository.Update(loanApplicationHistory);
                    scope.Complete();
                    return result;
                }

                loanApplicationHistory = new Repositories.Entities.LoanApplicationStateHistory
                {
                    CreationDate = DateTime.UtcNow,
                    Observations = updateLoanApplicationDto.Observations,
                    LoanApplicationId = updateLoanApplicationDto.LoanApplicationId,
                    StateId = (int)ApplicationStateHistoryEnum.RevisionDeExtracto
                };

                await _loanApplicationStateHistoryRepository.Create(loanApplicationHistory);
            }

            scope.Complete();
            return result;
        }
    }
}
