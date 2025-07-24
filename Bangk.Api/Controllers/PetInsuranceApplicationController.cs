using Bangk.Common.Dto;
using Bangk.Domain.Contract.PetInsuranceApplication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class PetInsuranceApplicationController : BaseController
    {
        private readonly ICreatePetInsuranceApplicationService _createPetInsuranceApplicationService;
   
        public PetInsuranceApplicationController(ICreatePetInsuranceApplicationService createPetInsuranceApplicationService)
        {
            _createPetInsuranceApplicationService = createPetInsuranceApplicationService;
        }

        [HttpPost("CreatePetInsuranceApplication")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<bool>>> CreatePetInsuranceApplication(PetInsuranceDto petEnsuranceDto)
        {
            var result = await _createPetInsuranceApplicationService.Execute(petEnsuranceDto);
            if (result.HasErrors)
            {
                return BadRequest(result);
            }

            return Ok(new
            {
                ClientInsuredId = result.Result
            });
        }
    }
}
