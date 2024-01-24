using Bangk.Common.Dto;
using Bangk.Domain.Contract.Costumer;
using Bangk.Domain.Implementation.Client;
using Bangk.Repositories.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class ClientController : BaseController
    {


        private readonly ICostumerService _getClientService;
        private readonly IClientinfoService _getClientInfoService;
        public ClientController(ICostumerService getClientService, IClientinfoService getClientInfoService)
        {
            _getClientService = getClientService;
            _getClientInfoService = getClientInfoService;
        }

        [HttpGet("GetClientService")]
        [Authorize(Roles = "asesor, superAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ClientDto>>> GetClientService(int currentPage, int pageSize, string searchValue) {
            
             
            var result = await _getClientService.Execute(currentPage, pageSize, searchValue); 
            return Ok(new 
            {
                Clients = result 
            }); }

       


        [HttpGet("GetClientInfoService")]
        [Authorize(Roles = "asesor, superAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ClientInfoDto>>> GetClientInfoService(int clientId)
        {

            
            var result = await _getClientInfoService.Execute(clientId);
            return Ok(new
            {
                ClientsInfo = result
            });
        }

    }
    
}
