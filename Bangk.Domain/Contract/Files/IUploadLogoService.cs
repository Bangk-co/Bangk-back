using Bangk.Common.Dto;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Files
{
    public interface IUploadFileService
    {
        Task<ResponseDto<string>> Execute(IFormFile file, string containerName, string fileName = null);
    }
}
