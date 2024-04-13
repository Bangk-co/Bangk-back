using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Bangk.Common.Dto;
using Bangk.Domain.Contract.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Files
{
    public class UploadFileService : IUploadFileService
    {
        private readonly IConfiguration _configuration;

        public UploadFileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public async Task<ResponseDto<string>> Execute(IFormFile file, string containerName, string fileName = null)
        {
            var result = new ResponseDto<string>();
            string connectionString = 
                $"DefaultEndpointsProtocol=https;AccountName={_configuration.GetSection("AzureStorage:StorageAccountName").Value};AccountKey={_configuration.GetSection("AzureStorage:StorageAccountKey").Value}";
            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(string.IsNullOrEmpty(fileName) ? file.FileName : fileName);

            // Upload data from the local file
            await blobClient.UploadAsync(file.OpenReadStream(), new BlobHttpHeaders { ContentType = file.ContentType });
            result.Result = blobClient.Uri.AbsoluteUri;
            result.HasErrors = false;

            return result;
        }
    }
}
