using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanInsurancePolicy;
using Bangk.Domain.Contract.LoanQuote;
using Bangk.Repositories.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class LoanInsurancePolicyController : BaseController
    {
        private readonly IGetAllLoanInsurancePolicyByLoanApplicationIdService _loanInsurancePolicyByLoanApplicationIdService;
        private readonly ICreateLoanInsurancePolicyService _createLoanInsurancePolicyService;
        private readonly IChangeStateLoanInsurancePolicyService _changeStateLoanInsurancePolicyService;
        public LoanInsurancePolicyController(IGetAllLoanInsurancePolicyByLoanApplicationIdService loanInsurancePolicyByLoanApplicationIdService,
            ICreateLoanInsurancePolicyService createLoanInsurancePolicyService, IChangeStateLoanInsurancePolicyService changeStateLoanInsurancePolicyService)
        {
            _loanInsurancePolicyByLoanApplicationIdService = loanInsurancePolicyByLoanApplicationIdService;
            _createLoanInsurancePolicyService = createLoanInsurancePolicyService;
            _changeStateLoanInsurancePolicyService = changeStateLoanInsurancePolicyService;
        }
      

        [HttpGet("GetAllLoanInsurancePolicyByLoanApplication")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLoanQuoteByApplicationId(int loanApplicationId)
        {
            var result = await _loanInsurancePolicyByLoanApplicationIdService.Execute(loanApplicationId);
            return Ok(new
            {
                LoanInsurancePolicies = result
            });
        }

        [HttpPost("CreateLoanInsurancePolicy")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateLoanInsurancePolicy(LoanInsurancePolicySaveDto loanInsurancePolicySaveDto)
        {
            if(loanInsurancePolicySaveDto.beneficiary.Count > 7)
            {
                return( BadRequest("Supero el limite de beneficiarios permitidos.") );
            }
            var result = await _createLoanInsurancePolicyService.Execute(loanInsurancePolicySaveDto);
            return Ok(result);
        }

        [HttpPost("ChangeStateLoanInsurancePolicy")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangeStateLoanInsurancePolicy(int loanApplicationId)
        {
            var result = await _changeStateLoanInsurancePolicyService.Execute(loanApplicationId);
            return Ok(result);
        }
    }
}
