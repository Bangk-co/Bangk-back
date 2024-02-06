using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class LoanApplicationController : BaseController
    {
        private readonly ILoanApplicationServices _getLoanApplicationServices;
        private readonly IGetLoadApplicationByIdService _getLoadApplicationByIdService;
        public LoanApplicationController(ILoanApplicationServices getLoanApplicationServices, IGetLoadApplicationByIdService getLoadApplicationByIdService)
        {
            _getLoanApplicationServices = getLoanApplicationServices;
            _getLoadApplicationByIdService = getLoadApplicationByIdService;
        }

        [HttpGet("GetLoanApplicationService")]
        [Authorize(Roles = "asesor, superAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<LoanApplicationDto>>> GetLoanApplicationService(int ClientId) 
        {
            
            var result = await _getLoanApplicationServices.Execute(ClientId);
            return Ok(new
            {
                loanApplications = result
            });
        }

        [HttpGet("GetLoanApplicationByApplicationId")]
        [Authorize(Roles = "asesor, superAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<LoanApplicationDto>>> GetLoanApplicationByApplicationId(int loanApplicationId)
        {
            var result = await _getLoadApplicationByIdService.Execute(loanApplicationId);
            return Ok(new
            {
                loanApplication = result
            });
        }
    }
}
