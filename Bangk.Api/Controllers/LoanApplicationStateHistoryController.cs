using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplicationStateHistory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class LoanApplicationStateHistoryController: BaseController
    {
        private readonly ILoanApplicationStateHistory _getLoanApplicationStateHistoryService;

        public LoanApplicationStateHistoryController(ILoanApplicationStateHistory getLoanApplicationStateHistoryService)
        {
            _getLoanApplicationStateHistoryService = getLoanApplicationStateHistoryService;
        }

        [HttpGet("GetLoanApplicationStateHistoryService")]
        [Authorize(Roles = "asesor, superAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult<LoanApplicationStateHistoryResultDto>> GetLoanApplicationStateHistoryService(int loanApplicationId)
        {
            var response = await _getLoanApplicationStateHistoryService.Execute(loanApplicationId);
            return Ok(response);
        }
    }
}
