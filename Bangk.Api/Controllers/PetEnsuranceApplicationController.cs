using Bangk.Common.Dto;
using Bangk.Domain.Contract.Client;
using Bangk.Domain.Contract.PetEnsuranceApplication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class PetEnsuranceApplicationController : BaseController
    {
        private readonly ICreatePetEnsuranceApplicationService _createPetEnsuranceApplicationService;
   
        public PetEnsuranceApplicationController(ICreatePetEnsuranceApplicationService createPetEnsuranceApplicationService)
        {
            _createPetEnsuranceApplicationService = createPetEnsuranceApplicationService;
        }

        [HttpPost("CreatePetEnsuranceApplication")]
        [AllowAnonymous]
        public async Task<ActionResult<List<MedPersonInsuredsDto>>> CreatePetEnsuranceApplication(PetEnsuranceDto petEnsuranceDto)
        {
            var result = await _createPetEnsuranceApplicationService.Execute(petEnsuranceDto);
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
