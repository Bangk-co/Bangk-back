using Bangk.Common.Dto;
using Bangk.Domain.Contract.MedPersonalInsureds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        public async Task<ActionResult<List<MedPersonInsuredsDto>>> CreateMedPersonalInsuredService(MedPersonInsuredsDto medPersonInsuredsDto)
        {
            var result = await _createMedPersonalInsuredService.Execute(medPersonInsuredsDto);
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
