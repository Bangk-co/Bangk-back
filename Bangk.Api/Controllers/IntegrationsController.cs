using Bangk.Common.Dto;
using Bangk.Domain.Contract.City;
using Bangk.Domain.Contract.Integrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class IntegrationsController : BaseController
    {
        private readonly IValidationSarlaftService _validationSarlaftService;
        private readonly IValidationSBSService _validationSBSService;
        public IntegrationsController(IValidationSarlaftService validationSarlaftService, IValidationSBSService validationSBSService)
        {
            _validationSBSService = validationSBSService;
            _validationSarlaftService = validationSarlaftService;
        }

        [HttpGet("ValidationSarlaft")]
        [AllowAnonymous]

        public async Task<ActionResult> ValidationSarlaft(int loanApplicationId)
        {
            await _validationSarlaftService.Execute(loanApplicationId, true);
            return Ok();
        }

        [HttpGet("ValidationSBSService")]
        [AllowAnonymous]

        public async Task<ActionResult> ValidationSBSService(int loanApplicationId)
        {
            await _validationSBSService.Execute(loanApplicationId, true);
            return Ok();
        }
    }
}
