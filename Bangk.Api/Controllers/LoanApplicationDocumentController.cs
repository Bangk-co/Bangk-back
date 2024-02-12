using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplicationDocument;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class LoanApplicationDocumentController : BaseController
    {
        private readonly IGetLoadApplicationDocumentService _getLoanApplicationDocumentServices;
        public LoanApplicationDocumentController(IGetLoadApplicationDocumentService getLoanApplicationDocuementServices)
        {
            _getLoanApplicationDocumentServices = getLoanApplicationDocuementServices;
        }

        [HttpGet("GetLoadApplicationDocumentByClient")]
        [Authorize(Roles = "asesor, superAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<LoanApplicationDto>>> GetLoadApplicationDocumentByClient(int ClientId) 
        {
            var result = await _getLoanApplicationDocumentServices.Execute(ClientId);
            return Ok(new
            {
                loanApplicationDocuments = result
            });
        }

    
    }
}
