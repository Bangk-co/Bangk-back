using Bangk.Common.Dto;
using Bangk.Domain.Contract.GroupSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class GroupSettingController : BaseController
    {
        private readonly IGetGroupSettingListService _getGroupSettingListService;
        public GroupSettingController(IGetGroupSettingListService getGroupSettingListService)
        {
            _getGroupSettingListService = getGroupSettingListService;
        }

        [HttpGet("GetGroupSettingList")]
        [AllowAnonymous]
        public async Task<ActionResult<List<GroupSettingListDto>>> GetGroupSettingList()
        {
            var result = await _getGroupSettingListService.Execute();
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
                GroupSettings = result
            });
        }
    }
}
