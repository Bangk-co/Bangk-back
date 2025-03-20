using Bangk.Common.Dto;
using Bangk.Common.Dto.MecharntsInfoDto;
using Bangk.Domain.Contract.Integrations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Integrations
{
    public class GetMerchantsTokenService : IGetMerchantsTokenService
    {
        private readonly IConfiguration _configuration;
        public GetMerchantsTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ResponseDto<MecharntsInfoDto>> Execute()
        {
            var response = new ResponseDto<MecharntsInfoDto>();
            var wompiUrl = _configuration.GetSection("WompiKeys:UrlWompipayment").Value;
            var publicKey = _configuration.GetSection("WompiKeys:PublicKey").Value;
            wompiUrl = $"{wompiUrl}merchants/{publicKey}";
            using (var client = new HttpClient())
            {
                var apiResponse = await client.GetAsync(wompiUrl);
                //Logs.WriteInfoLog($"Status Code API: {apiResponse.StatusCode}");
                if (apiResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await apiResponse.Content.ReadAsStringAsync();
                    var apiResponseBancolombia = Newtonsoft.Json.JsonConvert.DeserializeObject<MecharntsInfoDto>(jsonResponse);
                    //Logs.WriteInfoLog($"Response: {jsonResponse}");
                    response = new ResponseDto<MecharntsInfoDto>
                    {
                        HasErrors = false,
                        Result = apiResponseBancolombia
                    };
                }
                else
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    //Logs.WriteErrorLog($"Error: {errorMessage} - StatusCode: {apiResponse.StatusCode}");
                    response = new ResponseDto<MecharntsInfoDto>
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
