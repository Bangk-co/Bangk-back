using Bangk.Common.Dto;
using Bangk.Domain.Contract.MedPersonalInsureds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class MedPersonalInsuredController: BaseController
    {
        private readonly ICreateMedPersonalInsuredService _createMedPersonalInsuredService;

        public MedPersonalInsuredController(ICreateMedPersonalInsuredService createMedPersonalInsuredService)
        {
            _createMedPersonalInsuredService = createMedPersonalInsuredService;
        }

        [HttpPost("CreateMedPersonalInsuredService")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<int>>> CreateMedPersonalInsuredService( [FromBody] MedPersonInsuredsDto medPersonInsuredsDto)
        {
            if (medPersonInsuredsDto == null || medPersonInsuredsDto.BeneficiaryInsurance == null || !medPersonInsuredsDto.BeneficiaryInsurance.Any())
            {
                return BadRequest(new ResponseDto<int>
                {
                    HasErrors = true,
                    Errors = new List<string> { "Debe proporcionar al menos un beneficiario para el seguro." }
                });
            }

            var result = await _createMedPersonalInsuredService.Execute(medPersonInsuredsDto);

            if (result.HasErrors)
            {
                return BadRequest(result);
            }

            return Ok(new
            {
                MedEnsuranceApplicationId = result.Result
            });
        }

    }
}
