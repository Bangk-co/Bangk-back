using Bangk.Common.Dto;
using Bangk.Domain.Contract.City;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class CityController: BaseController
    {
        private readonly IGetCityService _cityService;

        public CityController (IGetCityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet("GetCitiesList")]
        [AllowAnonymous]

        public async Task<ActionResult> GetCitiesList(int stateId)
        {
            if (stateId == 0)
            {
                var error = new ResponseDto<bool>
                {
                    HasErrors = true,
                    Result = false
                };

                error.Errors.Add("El parametro de busqueda es obligatorio.");
                return BadRequest(error);
            }

            var (cities, stateName) = await _cityService.Execute(stateId);

            if (cities.Count == 0)
            {
                if (stateName != null)
                {
                    return Ok(new
                    {
                        StateName = stateName,
                        Cities = cities
                    });
                }
                else
                {
                    var error = new ResponseDto<bool>
                    {
                        HasErrors = true,
                        Result = false
                    };

                    error.Errors.Add("No hay lista creada y el estado no existe.");
                    return BadRequest(error);
                }
            }

            return Ok(new
            {
                Cities = cities
            });
        }
    }
}
