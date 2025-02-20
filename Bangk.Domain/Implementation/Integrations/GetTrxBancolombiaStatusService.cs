using Bangk.Common.Dto;
using Bangk.Domain.Contract.Integrations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Integrations
{
    public class GetTrxBancolombiaStatusService : IGetTrxBancolombiaStatusService
    {
        private readonly IConfiguration _configuration;

        public GetTrxBancolombiaStatusService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
            
        public async Task<ResponseDto<BancolombiaButtonTransferDto>> Execute(string id)
        {
            var response = new ResponseDto<BancolombiaButtonTransferDto>();
            var wompiUrl = _configuration.GetSection("WompiKeys:UrlWompipayment").Value;
            var publicKey = _configuration.GetSection("WompiKeys:PublicKey").Value;
            wompiUrl = $"{wompiUrl}tokens/bancolombia_transfer/{id}";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", publicKey);
                var apiResponse = await client.GetAsync(wompiUrl);
                //Logs.WriteInfoLog($"Status Code API: {apiResponse.StatusCode}");
                if (apiResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await apiResponse.Content.ReadAsStringAsync();
                    var apiResponseBancolombia = Newtonsoft.Json.JsonConvert.DeserializeObject<BancolombiaButtonTransferDto>(jsonResponse);
                    //Logs.WriteInfoLog($"Response: {jsonResponse}");
                    response = new ResponseDto<BancolombiaButtonTransferDto>
                    {
                        HasErrors = false,
                        Result = apiResponseBancolombia
                    };
                }
                else
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    //Logs.WriteErrorLog($"Error: {errorMessage} - StatusCode: {apiResponse.StatusCode}");
                    response = new ResponseDto<BancolombiaButtonTransferDto>
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
