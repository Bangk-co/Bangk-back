using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanQuote;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class LoanQuoteController : BaseController
    {
        private readonly ILoanQuoteService _loanQuoteService;

        public LoanQuoteController(ILoanQuoteService loanQuoteService)
        {
            _loanQuoteService = loanQuoteService;
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

    }
}
