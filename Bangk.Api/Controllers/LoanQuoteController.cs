using Bangk.Domain.Contract.LoanQuote;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class LoanQuoteController : BaseController
    {
        private readonly ILoanQuoteService _loanQuoteService;
        private readonly IGetLoanQuoteInfoByLoanApplicationIdService _getLoanQuoteInfoByLoanApplicationIdService;
        private readonly IAcceptLoanQuoteService _acceptLoanQuoteService;
        private readonly IRejectLoanQuoteService _rejectLoanQuoteService;
        public LoanQuoteController(ILoanQuoteService loanQuoteService, IGetLoanQuoteInfoByLoanApplicationIdService getLoanQuoteInfoByLoanApplicationIdService,
            IAcceptLoanQuoteService acceptLoanQuoteService, IRejectLoanQuoteService rejectLoanQuoteService)
        {
            _loanQuoteService = loanQuoteService;
            _getLoanQuoteInfoByLoanApplicationIdService = getLoanQuoteInfoByLoanApplicationIdService;
            _acceptLoanQuoteService = acceptLoanQuoteService;
            _rejectLoanQuoteService = rejectLoanQuoteService;
        }

        [HttpPost("CreateLoanQuote")]
        [Authorize(Roles = "asesor, superAdmin")]

        public async Task<IActionResult> CreateLoanQuote(int insuranceId, int loanApplicationId, long yearlyFee, string insuranceName)
        {
            int loanQuoteId = await _loanQuoteService.Execute(insuranceId, loanApplicationId, yearlyFee, insuranceName);
            if (loanQuoteId == 0) 
            {
                return BadRequest("No se pudo crear la cotización del préstamo.");
            }

            return Ok(loanApplicationId);
        }

        [HttpGet("GetLoanQuoteByApplicationId")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLoanQuoteByApplicationId(int loanApplicationId)
        {
            var result = await _getLoanQuoteInfoByLoanApplicationIdService.Execute(loanApplicationId);
            return Ok(result);
        }

        [HttpPost("AcceptLoanQuote")]
        [AllowAnonymous]
        public async Task<IActionResult> AcceptLoanQuote(int loanApplicationId, DateTime termsConditionsAcceptanceQuote)
        {
            var result = await _acceptLoanQuoteService.Execute(loanApplicationId, termsConditionsAcceptanceQuote);
            return Ok(result);
        }

        [HttpPost("RejectLoanQuote")]
        [AllowAnonymous]
        public async Task<IActionResult> RejectLoanQuote(int loanApplicationId, string observations)
        {
            var result = await _rejectLoanQuoteService.Execute(loanApplicationId, observations);
            return Ok(result);
        }
    }
}
