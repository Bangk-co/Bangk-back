using Bangk.Common;
using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.Integrations;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Integrations
{
    public class CreateBancolombiaButtonPaymentService : ICreateBancolombiaButtonPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IApplicationTransactionRepository _applicationTransactionRepository;

        public CreateBancolombiaButtonPaymentService(IConfiguration configuration, IApplicationTransactionRepository applicationTransactionRepository)
        {
            _configuration = configuration;
            _applicationTransactionRepository = applicationTransactionRepository;
        }
        public async Task<ResponseDto<Bangk.Common.Dto.BancolombiaButtonTransferResponseDto.BancolombiaButtonTransferResponseDto>> Execute()
        {
            var response = new ResponseDto<Bangk.Common.Dto.BancolombiaButtonTransferResponseDto.BancolombiaButtonTransferResponseDto>();
            var wompiUrl = _configuration.GetSection("WompiKeys:UrlWompipayment").Value;
            var publicKey = _configuration.GetSection("WompiKeys:PublicKey").Value;

            wompiUrl = $"{wompiUrl}tokens/bancolombia_transfer";
            var query = new
            {
                redirect_url = _configuration.GetSection("WompiKeys:RedirectBankgUrl").Value,
                type_auth = "TOKEN"
            };

            var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(query), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", publicKey);
                var apiResponse = await client.PostAsync(wompiUrl, jsonContent);
                //Logs.WriteInfoLog($"Status Code API: {apiResponse.StatusCode}");

                if (apiResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await apiResponse.Content.ReadAsStringAsync();
                    var apiResponseBancolombia = Newtonsoft.Json.JsonConvert.DeserializeObject<Bangk.Common.Dto.BancolombiaButtonTransferResponseDto.BancolombiaButtonTransferResponseDto>(jsonResponse);
                    //Logs.WriteInfoLog($"Response: {jsonResponse}");

                    var applicationToSave = new Repositories.Entities.ApplicationTransaction
                    {
                        ApplicationType = (int)ApplicationTypeEnum.BancolombiaButtonTransfer,
                        CreationDate = apiResponseBancolombia.Data.Created_At,
                        TransactionNumber = apiResponseBancolombia.Data.Id,
                        Status = apiResponseBancolombia.Data.Status,
                    };
                    await _applicationTransactionRepository.Create(applicationToSave);

                    response = new ResponseDto<Common.Dto.BancolombiaButtonTransferResponseDto.BancolombiaButtonTransferResponseDto>
                    {
                        HasErrors = false,
                        Result = apiResponseBancolombia
                    };
                }
                else
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    //Logs.WriteErrorLog($"Error: {errorMessage} - StatusCode: {apiResponse.StatusCode}");
                    response = new ResponseDto<Common.Dto.BancolombiaButtonTransferResponseDto.BancolombiaButtonTransferResponseDto>
                    {
                        HasErrors = true,
                        Errors = new List<string> { $"Se ha generado un error procesando la solicitud: {errorMessage}" }
                    };
                }
            }

            return response;
        }
    }
}
