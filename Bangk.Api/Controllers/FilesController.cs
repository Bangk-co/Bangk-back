using Bangk.Common.Dto;
using Bangk.Domain.Contract.Files;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class FilesController : BaseController
    {
        private readonly IUploadFileService _uploadFileService;
        private readonly IConfiguration _configuration;
        public FilesController(IUploadFileService uploadFileService, IConfiguration configuration)
        {
            _uploadFileService = uploadFileService;
            _configuration = configuration;
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

            var result = await _uploadFileService.Execute(file, _configuration.GetSection("AzureStorage:ContainerName").Value);
            if (result.HasErrors)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
