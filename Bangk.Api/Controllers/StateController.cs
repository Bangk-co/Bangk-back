using Bangk.Common.Dto;
using Bangk.Domain.Contract.State;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class StateController: BaseController
    {
        private readonly IGetStatesService _getstatesService;

        public StateController(IGetStatesService getstatesService)
        {
            _getstatesService = getstatesService;
        }

        [HttpGet("GetStateList")]
        [AllowAnonymous]

        public async Task<ActionResult<List<StateDto>>> GetStateList(int countryId)
        {
            if (countryId == 0)
            {
                var error = new ResponseDto<bool>
                {
                    HasErrors = true,
                    Result = false
                };

                error.Errors.Add("El parametro de busqueda es obligatorio.");
                return BadRequest(error);
            }

            var result = await _getstatesService.Execute(countryId);
            if (result.Count == 0)
            {
                var error = new ResponseDto<bool>
                {
                    HasErrors = true,
                    Result = false
                };

                error.Errors.Add("No hay lista creada.");
                return BadRequest(error);
            }

            return Ok(new
            {
                States = result
            });
        }

    }
}
