using Bangk.Common.Dto;
using Bangk.Domain.Contract.GEA;
using Bangk.Repositories.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.GEA
{
    public class GetApplicationTokenGEAService : IGetApplicationTokenGEAService
    {
        private readonly IConfiguration _configuration;

        public GetApplicationTokenGEAService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ResponseDto<string>> Execute()
        {
            var baseUrl = _configuration.GetSection("GeaKeys:GeaUrl").Value;
            var response = new ResponseDto<string>();
            using (HttpClient client = new HttpClient())
            {
                var loginParam = new GEALoginDto
                {
                    client_id = int.Parse(_configuration.GetSection("GeaKeys:ClientId").Value),
                    client_secret = _configuration.GetSection("GeaKeys:ClientSecret").Value
                };
                var json = JsonConvert.SerializeObject(loginParam);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage jsonResponse = await client.PostAsync($"{baseUrl}v1/auth/token", content);
                string result = await jsonResponse.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<dynamic>(result);

                if (!jsonResponse.IsSuccessStatusCode)
                {
                    response = new ResponseDto<string>
                    {
                        HasErrors = true,
                        Errors = new List<string> { $"Se ha generado un error procesando la solicitud" }
                    };
                }

                var token = apiResponse["data"]["access_token"];
                response = new ResponseDto<string>
                {
                    HasErrors = false,
                    Result = token
                };
            }

            return response;
        }
    }
}
