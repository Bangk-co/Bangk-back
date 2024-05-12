using Bangk.Common.Dto;
using Bangk.Domain.Contract.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class UserController : BaseController
    {
        private readonly IGetUserInfoByClientIdService _getUserInfoByClientIdService;

        public UserController(IGetUserInfoByClientIdService getUserInfoByClientIdService)
        {
            _getUserInfoByClientIdService = getUserInfoByClientIdService;
        }

        [HttpGet("GetUserInfoByClientId")]
        [AllowAnonymous]

        public async Task<ActionResult<List<CityDto>>> GetUserInfoByClientId(int clientId)
        {
            if (clientId == 0)
            {
                var error = new ResponseDto<bool>
                {
                    HasErrors = true,
                    Result = false
                };

                error.Errors.Add("El parametro de busqueda es obligatorio.");
                return BadRequest(error);
            }

            var result = await _getUserInfoByClientIdService.Execute(clientId);
            return Ok(result);
        }
    }
}
