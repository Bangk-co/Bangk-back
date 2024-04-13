using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplicationDocument;
using Bangk.Domain.Implementation.LoanApplicationDocument;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class LoanApplicationDocumentController : BaseController
    {
        private readonly IGetLoadApplicationDocumentService _getLoanApplicationDocumentServices;
        private readonly IGetDownloadDocumentService _getDownloadDocumentServices;
        public LoanApplicationDocumentController(IGetLoadApplicationDocumentService getLoanApplicationDocuementServices, IGetDownloadDocumentService getDownloadDocumentService)
        {
            _getLoanApplicationDocumentServices = getLoanApplicationDocuementServices;
            _getDownloadDocumentServices = getDownloadDocumentService;

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

        [HttpGet("GetDownloadDocument")]
        [Authorize(Roles = "asesor, superAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            try
            {
                var downloadDocumentDto = await _getDownloadDocumentServices.Execute(documentId);

                if (downloadDocumentDto == null)
                {
                    return NotFound();
                }

                return Redirect(downloadDocumentDto.Url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al descargar el documento: {ex.Message}");
                return StatusCode(500);
            }
        }

    }
}
