using Bangk.Domain.Contract.LoanQuote;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class LoanQuoteController : BaseController
    {
        private readonly ILoanQuoteService _loanQuoteService;
        private readonly IGetLoanQuoteInfoByLoanApplicationIdService _getLoanQuoteInfoByLoanApplicationIdService;
        private readonly IAcceptLoanQuoteService _acceptLoanQuoteService;
        public LoanQuoteController(ILoanQuoteService loanQuoteService, IGetLoanQuoteInfoByLoanApplicationIdService getLoanQuoteInfoByLoanApplicationIdService,
            IAcceptLoanQuoteService acceptLoanQuoteService)
        {
            _loanQuoteService = loanQuoteService;
            _getLoanQuoteInfoByLoanApplicationIdService = getLoanQuoteInfoByLoanApplicationIdService;
            _acceptLoanQuoteService = acceptLoanQuoteService;
        }

        [HttpPost("CreateLoanQuote")]
        [Authorize(Roles = "asesor, superAdmin")]
        
        public async Task<IActionResult> CreateLoanQuote(int insuranceId, int loanApplicationId, long yearlyFee, string observations)
        {
            int loanQuoteId = await _loanQuoteService.Execute(insuranceId, loanApplicationId, yearlyFee, observations);
            if (loanQuoteId == 0) 
            {
                return BadRequest("No se pudo crear la cotización del préstamo.");
            }

            return Ok(loanQuoteId);
        }

        [HttpPost("GetLoanQuoteByApplicationId")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLoanQuoteByApplicationId(int loanApplicationId)
        {
            var result = await _getLoanQuoteInfoByLoanApplicationIdService.Execute(loanApplicationId);
            return Ok(result);
        }

        [HttpPost("AcceptLoanQuote")]
        [AllowAnonymous]
        public async Task<IActionResult> AcceptLoanQuote(int loanApplicationId)
        {
            var result = await _acceptLoanQuoteService.Execute(loanApplicationId);
            return Ok(result);
        }
    }
}
