using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplication;
using Bangk.Domain.Implementation.LoanApplication;
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
        private readonly IUpdateLoanApplicationService _updateLoanApplicationService;
        private readonly IGetLoanApplicationInfoByLoanApplicationIdService _getLoanApplicationInfoByLoanApplicationIdService;
        private readonly IGetLoanApplicationFirstThreeService _getLoanApplicationFirstThreeService; 
        public LoanApplicationController(ILoanApplicationServices getLoanApplicationServices, IGetLoadApplicationByIdService getLoadApplicationByIdService, 
            IUpdateLoanApplicationService updateLoanApplicationService, IGetLoanApplicationInfoByLoanApplicationIdService getLoanApplicationInfoByLoanApplicationIdService,
            IGetLoanApplicationFirstThreeService getLoanApplicationFirstThreeService)
        {
            _getLoanApplicationServices = getLoanApplicationServices;
            _getLoadApplicationByIdService = getLoadApplicationByIdService;
            _updateLoanApplicationService = updateLoanApplicationService;
            _getLoanApplicationInfoByLoanApplicationIdService = getLoanApplicationInfoByLoanApplicationIdService;
            _getLoanApplicationFirstThreeService = getLoanApplicationFirstThreeService;
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
        public async Task<ActionResult<LoanApplicationInfoDto>> GetLoanApplicationByApplicationId(int loanApplicationId)
        {
            var result = await _getLoadApplicationByIdService.Execute(loanApplicationId);
            return Ok(new
            {
                loanApplication = result
            });
        }

        [HttpPut("UpdateLoanApplicationDto")]
        [Authorize(Roles = "asesor, superAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<bool>> UpdateLoanApplicationDto(UpdateLoanApplicationDto updateLoanApplicationDto)
        {
            var result = await _updateLoanApplicationService.Execute(updateLoanApplicationDto);
            return Ok(new
            {
                result
            });
        }

        [HttpGet("GetLoanApplicationByLoanApplicationId")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<LoanApplicationDto>>> GetLoanApplicationByLoanApplicationId(int loanApplicationId)
        {
            var result = await _getLoanApplicationInfoByLoanApplicationIdService.Execute(loanApplicationId);
            return Ok(new
            {
                result
            });
        }

        [HttpGet("GetLoanApplicationFirstThree")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<List<ClientPolicyDto>> GetLoanApplicationFirstThree(int clientId)
        {
            var result = _getLoanApplicationFirstThreeService.Execute(clientId);
            return Ok(new
            {
                loanApplications = result
            });
        }
    }
}
