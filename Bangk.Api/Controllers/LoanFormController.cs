using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanForm;
using Bangk.Domain.Implementation.LoanForm;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class LoanFormController: BaseController
    {
        private readonly ILoanSaveFormService _loanSaveFormService;
        private readonly IGetLoanFormService _getLoanFormService;

        public LoanFormController(ILoanSaveFormService loanSaveFormService, IGetLoanFormService getLoanFormService)
        {
            _loanSaveFormService = loanSaveFormService;
            _getLoanFormService = getLoanFormService;
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


        [HttpGet("GetLoanForm")]
        [Authorize(Roles = "asesor, superAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<LoanFormDto>>> GetLoanForm(int loanApplicationId)
        {
            if (loanApplicationId == 0)
            {
                var error = new ResponseDto<bool>
                {
                    HasErrors = true,
                    Result = false
                };

                error.Errors.Add("El parametro de busqueda es obligatorio.");
                return BadRequest(error);
            }

            var result = await _getLoanFormService.Execute(loanApplicationId);
            if (result.Count == 0)
            {
                var error = new ResponseDto<bool>
                {
                    HasErrors = true,
                    Result = false
                };

                error.Errors.Add("No existen comentarios para esta solicitud.");
                return BadRequest(error);
            }

            return Ok(new
            {
                GetLoanForm = result
            });
        }
    }
}
