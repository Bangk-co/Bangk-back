using Bangk.Domain.Contract.LoanQuote;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class LoanInsurancePolicyController : BaseController
    {
        private readonly IGetAllLoanInsurancePolicyByLoanApplicationIdService _loanInsurancePolicyByLoanApplicationIdService;
        public LoanInsurancePolicyController(IGetAllLoanInsurancePolicyByLoanApplicationIdService loanInsurancePolicyByLoanApplicationIdService)
        {
            _loanInsurancePolicyByLoanApplicationIdService = loanInsurancePolicyByLoanApplicationIdService;
        }
      

        [HttpGet("GetAllLoanInsurancePolicyByLoanApplication")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLoanQuoteByApplicationId(int loanApplicationId)
        {
            var result = await _loanInsurancePolicyByLoanApplicationIdService.Execute(loanApplicationId);
            return Ok(result);
        }
    }
}
