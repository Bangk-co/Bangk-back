using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.Integrations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bangk.Domain.Contract.Integrations.Wompi;

namespace Bangk.Domain.Implementation.Integrations
{
    public class CreateTransactionBancolombiaService : ICreateTransactionBancolombiaService
    {
        private readonly IConfiguration _configuration;
        private readonly IGenerateIntegrityKeyService _generateIntegrityKeyService;
        public CreateTransactionBancolombiaService(IConfiguration configuration, IGenerateIntegrityKeyService generateIntegrityKeyService)
        {
            _configuration = configuration;
            _generateIntegrityKeyService = generateIntegrityKeyService;
        }

        public async Task<ResponseDto<string>> Execute(int value, int paymentSourceId)
        {
            var response = new ResponseDto<string>();
            var wompiUrl = _configuration.GetSection("WompiKeys:UrlWompipayment").Value;
            var privateKey = _configuration.GetSection("WompiKeys:PrivateKey").Value;
            var integrityKey = await _generateIntegrityKeyService.Execute(value, Guid.NewGuid().ToString("N"));

            wompiUrl = $"{wompiUrl}transactions";
            var query = new
            {
                amount_in_cents = value, // Monto current centavos
                currency = "COP", // Moneda
                signature = integrityKey, //Firma de integridad
                customer_email = "example@gmail.com", // Email del usuario
                reference = Guid.NewGuid().ToString("N"), // Referencia única de pago
                payment_source_id = paymentSourceId
            };

            var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(query), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", privateKey);
                var apiResponse = await client.PostAsync(wompiUrl, jsonContent);
                //Logs.WriteInfoLog($"Status Code API: {apiResponse.StatusCode}");

                if (apiResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await apiResponse.Content.ReadAsStringAsync();
                    //var apiResponseBancolombia = Newtonsoft.Json.JsonConvert.DeserializeObject<Bangk.Common.Dto.BancolombiaButtonTransferResponseDto.BancolombiaButtonTransferResponseDto>(jsonResponse);
                    //Logs.WriteInfoLog($"Response: {jsonResponse}");

                    response = new ResponseDto<string>
                    {
                        HasErrors = false,
                        Result = jsonResponse
                    };
                }
                else
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    //Logs.WriteErrorLog($"Error: {errorMessage} - StatusCode: {apiResponse.StatusCode}");
                    response = new ResponseDto<string>
                    {
                        HasErrors = true,
                        Errors = new List<string> { errorMessage }
                    };
                }
            }

            return response;
        }
    }
}
