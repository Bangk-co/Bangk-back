using Bangk.Common.Dto;
using Bangk.Domain.Contract.Integrations;
using Bangk.Domain.Contract.Integrations.Wompi;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Integrations.Wompi
{
    public class CardPaymentService : ICardPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IGetMerchantsTokenService _getMerchantsTokenService;
        private readonly IGenerateIntegrityKeyService _generateIntegrityKeyService;

        public CardPaymentService(IConfiguration configuration, IGetMerchantsTokenService getMerchantsTokenService, IGenerateIntegrityKeyService generateIntegrityKeyService)
        {
            _configuration = configuration;
            _getMerchantsTokenService = getMerchantsTokenService;
            _generateIntegrityKeyService = generateIntegrityKeyService;
        }
        public async Task<ResponseDto<bool>> Execute(CardPaymentDto cardPaymentDto)
        {
            var response = new ResponseDto<bool>();
            var baseUrl = _configuration.GetSection("WompiKeys:UrlWompipayment").Value;
            var publicKey = _configuration.GetSection("WompiKeys:PublicKey").Value;
            var privateKey = _configuration.GetSection("WompiKeys:PrivateKey").Value;
            var wompiUrl = $"{baseUrl}tokens/cards";
            var paymentSourceUrl = $"{baseUrl}payment_sources"; 
            var acceptanceTokenUrl = $"{baseUrl}merchants/{publicKey}"; 
            var card = new CardDto
            {
                card_holder = cardPaymentDto.CardHolder,
                cvc = cardPaymentDto.CVC,
                exp_month = cardPaymentDto.ExpiredMonth,
                exp_year = cardPaymentDto.ExpiredYear,
                number = cardPaymentDto.CardNumber
                
            };

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {publicKey}");
                var json = JsonConvert.SerializeObject(card);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage jsonResponse = await client.PostAsync(wompiUrl, content);
                string result = await jsonResponse.Content.ReadAsStringAsync();
                var apiResponseCard = JsonConvert.DeserializeObject<CardTokenResponseDto>(result);

                if(!jsonResponse.IsSuccessStatusCode && apiResponseCard.Status != "CREATED")
                {
                    response = new ResponseDto<bool>
                    {
                        HasErrors = true,
                        Errors = new List<string> { $"Se ha generado un error procesando la solicitud" }
                    };
                }

                //Acceptance_Token"
                var merchantsInfo = await _getMerchantsTokenService.Execute();
                var acceptance_token = merchantsInfo.Result.Data.Presigned_Acceptance.Acceptance_Token;
                var accept_personal_auth = merchantsInfo.Result.Data.Presigned_Personal_Data_Auth.Acceptance_Token;

                //Payment Source
                var paymentSource = new
                {
                    type = "CARD",
                    token = apiResponseCard.Data.Id, // Token obtenido en el paso anterior
                    customer_email = cardPaymentDto.Email,
                    acceptance_token = acceptance_token,
                    accept_personal_auth = accept_personal_auth,
                };

                json = JsonConvert.SerializeObject(paymentSource);
                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {privateKey}");
                content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage responsePaymentSource = await client.PostAsync(paymentSourceUrl, content);
                result = await responsePaymentSource.Content.ReadAsStringAsync();
                var apiResponsePaymentSource = JsonConvert.DeserializeObject<CardPaymentSourceDto>(result);

                if (!responsePaymentSource.IsSuccessStatusCode && apiResponsePaymentSource.Data.Status != "AVAILABLE")
                {
                    response = new ResponseDto<bool>
                    {
                        HasErrors = true,
                        Errors = new List<string> { $"Se ha generado un error procesando la solicitud" }
                    };
                }

                //Transaction
                var signature = await _generateIntegrityKeyService.Execute(cardPaymentDto.Amount, Guid.NewGuid().ToString("N"));
            }

            return response;
        }
    }
}
