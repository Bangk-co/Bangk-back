using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.LoanInsurancePolicy;
using Bangk.Repositories.Repositories;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Client
{
    public class ChangeStateLoanInsurancePolicyService : IChangeStateLoanInsurancePolicyService
    {
        private readonly ILoanInsurancePolicyRepository _loanInsurancePolicyRepository;
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;


        public ChangeStateLoanInsurancePolicyService(ILoanInsurancePolicyRepository loanInsurancePolicyRepository, ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository)
        {
            _loanInsurancePolicyRepository = loanInsurancePolicyRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
        }

        public async Task<ResponseDto<bool>> Execute(int loanApplicationId)
        {
            var result = new ResponseDto<bool>
            {
                Result = true
            };

            var loanInsurencePolicy = await _loanInsurancePolicyRepository.GetLoanInsurancePolicyByLoadApplicationId(loanApplicationId);
            if (loanInsurencePolicy == null)
            {
                result.HasErrors = true;
                result.Errors.Add($"Aplicacion Id no existe.");
                result.Result = false;
                return result;
            }

            var observation = string.Empty;
            if (string.IsNullOrEmpty(loanInsurencePolicy.Diseases))
            {
               observation = $"Riesgo de salud presente {loanInsurencePolicy.DiseasesMoreInfo}"; 
            }

            if (loanInsurencePolicy.Disability.HasValue && loanInsurencePolicy.Disability.Value)
            {
                observation += !string.IsNullOrEmpty(observation) ?  $",Riesgo de salud presente {loanInsurencePolicy.DisabilityMoreInfo}" : $"Riesgo de salud presente {loanInsurencePolicy.DisabilityMoreInfo}";
            }

            if (loanInsurencePolicy.PlannedMedicalProcidure.HasValue && loanInsurencePolicy.PlannedMedicalProcidure.Value)
            {
                observation += !string.IsNullOrEmpty(observation) ? $",Riesgo de salud presente {loanInsurencePolicy.PlannedMedicalProcidureMoreInfo}" : $"Riesgo de salud presente {loanInsurencePolicy.PlannedMedicalProcidureMoreInfo}";
            }

            var loadApplicationStateHistory = new Repositories.Entities.LoanApplicationStateHistory
            {
                LoanApplicationId = loanApplicationId,
                Observations = observation,
                StateId = (int)ApplicationStateHistoryEnum.FormulariosDiligenciados,
            };

            await _loanApplicationStateHistoryRepository.Create(loadApplicationStateHistory);
            return result;
        }
    }
}
