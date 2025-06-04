using Bangk.Domain.Contract.Integrations;
using Bangk.Domain.Contract.LoanApplicationStateHistory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class IntegrationsController : BaseController
    {
        private readonly IValidationSarlaftService _validationSarlaftService;
        private readonly IValidationSBSService _validationSBSService;
        private readonly IAddCommentService _addCommentService;
        public IntegrationsController(IValidationSarlaftService validationSarlaftService, IValidationSBSService validationSBSService, IAddCommentService addCommentService)
        {
            _validationSBSService = validationSBSService;
            _validationSarlaftService = validationSarlaftService;
            _addCommentService = addCommentService;
        }

        [HttpPost("ValidationSarlaft")]
        [AllowAnonymous]

        public async Task<IActionResult> ValidationSarlaft(int loanApplicationId)
        {
            await _validationSarlaftService.Execute(loanApplicationId, true);
            return Ok();
        }

        [HttpPost("ValidationSBS")]
        public async Task<IActionResult> ValidationSBS(int loanApplicationId)
        {
            await _validationSBSService.Execute(loanApplicationId, true);
            return Ok();
        }


        [HttpPost("AddCommentToLoanApplicationHistory")]
        public async Task<IActionResult> AddCommentToLoanApplicationHistory(int historicalLoanApplicationId, string comment)
        {
            var result = await _addCommentService.Execute(historicalLoanApplicationId, comment);
            return Ok(result);
        }
    }
}
