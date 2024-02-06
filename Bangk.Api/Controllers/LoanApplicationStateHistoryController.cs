using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.LoanApplication;
using Bangk.Domain.Contract.LoanApplicationStateHistory;
using Bangk.Domain.Implementation.LoanApplication;
using Bangk.Domain.Implementation.LoanApplicationStateHistory;
using Bangk.Repositories.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    [AllowAnonymous]
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

            var resultQuery = await _getLoanApplicationStateHistoryService.Execute(loanApplicationId);
            var response = new LoanApplicationStateHistoryResultDto();

            List<string> nameStates = new List<string>();
            foreach (var state in Enum.GetValues(typeof(ApplicationStateHistoryEnum)))
            {
                nameStates.Add(state.ToString());
            }
            response.States = new List<string>();
            response.States.AddRange(nameStates);

            response.LoanApplicationHistory = resultQuery;

            return Ok(response);
        }


    }
}
