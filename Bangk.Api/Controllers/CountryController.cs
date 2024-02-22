using Bangk.Common.Dto;
using Bangk.Domain.Contract.Country;
using Bangk.Domain.Contract.GroupSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class CountryController: BaseController
    {
        private readonly IGetCountryService _getCountryService;
        private readonly IGetNationalityService _getNationalityService;
        public CountryController(IGetCountryService getCountryService, IGetNationalityService getNationalityService )
        {
            _getCountryService = getCountryService;
            _getNationalityService = getNationalityService;
        }

        [HttpGet("GetCountriesList")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CountryDto>>> GetCountriesList()
        {
            var result = await _getCountryService.Execute();
            if (result.Count == 0)
            {
                var error = new ResponseDto<bool>
                {
                    HasErrors = true,
                    Result = false
                };

                error.Errors.Add("La lista no existe.");
                return BadRequest(error);
            }

            return Ok(new
            {
                Countries = result
            });
        }



        [HttpGet("GetNationalityList")]
        [AllowAnonymous]
        public async Task<ActionResult<List<NationalityDto>>> GetNationalityList()
        {
            var result = await _getNationalityService.Execute();
            if (result.Count == 0)
            {
                var error = new ResponseDto<bool>
                {
                    HasErrors = true,
                    Result = false
                };

                error.Errors.Add("La lista no existe.");
                return BadRequest(error);
            }

            return Ok(new
            {
                Countries = result
            });
        }



    }
}
