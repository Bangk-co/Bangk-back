using Bangk.Common.Dto;
using Bangk.Domain.Contract.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class SettingController : BaseController
    {
        private readonly IGetSettingListService _getSettingListService;
        public SettingController(IGetSettingListService getSettingListService)
        {
            _getSettingListService = getSettingListService;
        }

        [HttpGet("GetSettingList")]
        [AllowAnonymous]
        public async Task<ActionResult<List<SettingListDto>>> GetSettingList(int groupSettingId, string searchText)
        {
            if (groupSettingId == 0)
            {
                var error = new ResponseDto<bool>
                {
                    HasErrors = true,
                    Result = false
                };

                error.Errors.Add("El parametro de busqueda es obligatorio.");
                return BadRequest(error);
            }

            var result = await _getSettingListService.Execute(groupSettingId, searchText);
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
                GroupSettings = result
            });
        }
    }
}
