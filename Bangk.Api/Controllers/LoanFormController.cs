using Bangk.Domain.Contract.LoanForm;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class LoanFormController: BaseController
    {
        private readonly ILoanSaveFormService _loanSaveFormService;

        public LoanFormController(ILoanSaveFormService loanSaveFormService)
        {
            _loanSaveFormService = loanSaveFormService;
        }


        [HttpPost("SaveLoanForm")]
        [Authorize(Roles = "asesor, superAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SaveLoanForm(int loanApplicationId, string formName, string concept, string comment)
        {
            await _loanSaveFormService.Execute(loanApplicationId, formName, concept, comment);

            return Ok("Comentario guardado correctamente.");
        }
    }
}
