using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.LoanForm;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanForm
{
    public class ChangeStateFormsValidatedService: IChangeStateFormsValidatedService
    {
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        private readonly ILoanApplicationRepository _loanApplicationRepository;

        public ChangeStateFormsValidatedService(ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository, ILoanApplicationRepository loanApplicationRepository)
        {
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _loanApplicationRepository = loanApplicationRepository;
        }

        public async Task<ResponseDto<bool>> Execute(int loanApplicationId, string observation)
        {
            var loanApplication = _loanApplicationRepository.GetById(loanApplicationId);

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
                Observations = observation,
                StateId = (int)ApplicationStateHistoryEnum.FormulariosValidados
            };

            await _loanApplicationStateHistoryRepository.Create(loanApplicationHistory);
            return result;

        }
    }
}
