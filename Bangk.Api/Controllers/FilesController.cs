using Bangk.Common.Dto;
using Bangk.Domain.Contract.Files;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class FilesController : BaseController
    {
        private readonly IUploadFileService _uploadFileService;

        public FilesController(IUploadFileService uploadFileService)
        {
            _uploadFileService = uploadFileService;
        }

        [HttpPost("UploadFile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null)
            {
                var error = new ResponseDto<bool>
                {
                    HasErrors = true,
                    Result = false
                };

                error.Errors.Add("El archivo a subir es obligatorio.");
                return BadRequest(error);
            }

            var result = await _uploadFileService.Execute(file);
            if (result.HasErrors)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
