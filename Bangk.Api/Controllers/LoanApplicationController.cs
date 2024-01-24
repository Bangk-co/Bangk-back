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
        public LoanApplicationController(ILoanApplicationServices getLoanApplicationServices)
        {
            _getLoanApplicationServices = getLoanApplicationServices;
        }

        [HttpGet("GetLoanApplicationService")]
        [Authorize(Roles = "asesor")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<ActionResult<List<LoanApplicationDto>>> GetLoanApplicationService(int ClientId) 
        {
            
            var result = await _getLoanApplicationServices.Execute(ClientId);
            return Ok(new
            {
                Projects = result
            });
        }

    
    }
}
