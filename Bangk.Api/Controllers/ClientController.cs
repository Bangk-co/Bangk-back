using Bangk.Common.Dto;
using Bangk.Domain.Contract.Costumer;
using Bangk.Domain.Implementation.Client;
using Bangk.Repositories.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    
    public class ClientController : BaseController
    {

        private readonly ICostumerService _getClientService;
        public ClientController(ICostumerService getClientService)
        {
            _getClientService = getClientService;
        }

        [HttpGet("GetClientService")]
        [Authorize(Roles = "asesor")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ClientDto>>> GetClientService(int currentPage, int pageSize, string searchValue) {
            
            var identity = HttpContext.User.Identity as ClaimsIdentity; 
            var role = identity.Claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault()?.Value; 
            var userId = identity.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value; 
            var result = await _getClientService.Execute(currentPage, pageSize, searchValue); 
            return Ok(new 
            {
                Projects = result 
            }); }
    }
}
