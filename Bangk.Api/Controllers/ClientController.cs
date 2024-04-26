using Bangk.Common.Dto;
using Bangk.Domain.Contract.Client;
using Bangk.Domain.Contract.Costumer;
using Bangk.Domain.Contract.Users;
using Bangk.Repositories.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class ClientController : BaseController
    {
        private readonly ICostumerService _getClientService;
        private readonly IClientInfoService _getClientInfoService;
        private readonly ICreateClientService _createClientService;
        private readonly IUpdateClientService _updateClientService;
        private readonly ICreateUserService _createUserService;
        private readonly ICreateClientNotLoanApplicationService _createClientNotLoanApplicationService;
        private readonly IUpdateProfileService _updateProfileService;

        public ClientController(ICostumerService getClientService, IClientInfoService getClientInfoService, ICreateClientService createClientService,
            IUpdateClientService updateClientService, ICreateUserService createUserService, ICreateClientNotLoanApplicationService createClientNotLoanApplicationService, 
            IUpdateProfileService updateProfileService)
        {
            _getClientService = getClientService;
            _getClientInfoService = getClientInfoService;
            _createClientService = createClientService;
            _updateClientService = updateClientService;
            _createUserService = createUserService;
            _createClientNotLoanApplicationService = createClientNotLoanApplicationService;
            _updateProfileService = updateProfileService;
        }

        [HttpGet("GetClientService")]
        [Authorize(Roles = "asesor, superAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<ClientDto>>> GetClientService(int currentPage, int pageSize, string searchValue, string sortField, string sortOrder)
        {
            var result = await _getClientService.Execute(currentPage, pageSize, searchValue, sortField, sortOrder, User);
            return Ok(new
            {
                Clients = result
            });
        }

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

        [HttpPost("CreateClientService")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ClientDto>>> CreateClientService([FromForm] CreateClientDto createClientDto)
        {
            var result = await _createClientService.Execute(createClientDto);
            if (result.HasErrors)
            {
                return BadRequest(result);
            }

            return Ok(new
            {
                ClientId = result.Result
            });
        }

        [HttpPut("UpdateClientService")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateClient(int clientId, ClientUpdateInfoDto updatedClient)
        {
            var result = await _updateClientService.Execute(clientId, updatedClient);
            if (result.HasErrors)
            {
                return BadRequest(result);
            }

            return Ok(new
            {
                ClientUpdate = result.Result
            });
        }

        [HttpPost("CreateUserPassword")]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> CreateUserPassword(CreateUserDto createUser)
        {
            var result = await _createUserService.Execute(createUser);
            if (result.HasErrors)
            {
                return BadRequest(result);
            }

            return Ok(result.Result);
        }



        [HttpPost("CreateClient")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CreateClientNotLoanApplicationDto>>> CreateClient(CreateClientNotLoanApplicationDto createClientDto)
        {
            var result = await _createClientNotLoanApplicationService.Execute(createClientDto);
            if (result.HasErrors)
            {
                return BadRequest(result);
            }

            return Ok(new
            {
                result.Result
            });
        }

        [HttpPut("UpdateProfile")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto updatedClient)
        {
            var result = await _updateProfileService.Execute(updatedClient);
            if (result.HasErrors)
            {
                return BadRequest(result);
            }

            return Ok(new
            {
                result.Result
            });
        }
    }
}
