using Bangk.Common.Dto;
using Bangk.Domain.Contract.Integrations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Bangk.Repositories.Repositories;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;

namespace Bangk.Domain.Implementation.Integrations
{
    public class PaymentSourcesBancolombiaTransferService : IPaymentSourcesBancolombiaTransferService
    {
        private readonly IConfiguration _configuration;
        private readonly IGetMerchantsTokenService _getMerchantsTokenService;
        private readonly IApplicationTransactionRepository _applicationTransactionRepository;
        private readonly ICreateTransactionBancolombiaService _createTransactionBancolombiaService;

        public PaymentSourcesBancolombiaTransferService(IConfiguration configuration,
                                                        IGetMerchantsTokenService getMerchantsTokenService,
                                                        IApplicationTransactionRepository applicationTransactionRepository,
                                                        ICreateTransactionBancolombiaService createTransactionBancolombiaService)
        {
            _configuration = configuration;
            _getMerchantsTokenService = getMerchantsTokenService;
            _applicationTransactionRepository = applicationTransactionRepository;
            _createTransactionBancolombiaService = createTransactionBancolombiaService;
        }

        public async Task<ResponseDto<PaymentButtonBancolombiaResponseDto>> Execute(UserPaymentRequest userPaymentRequest)
        {
            var response = new ResponseDto<PaymentButtonBancolombiaResponseDto>();

            var merchantsInfo = await _getMerchantsTokenService.Execute();

            var wompiUrl = _configuration.GetSection("WompiKeys:UrlWompipayment").Value;
            var privateKey = _configuration.GetSection("WompiKeys:PrivateKey").Value;

            wompiUrl = $"{wompiUrl}payment_sources";

            var query = new
            {
                type = "BANCOLOMBIA_TRANSFER",
                token = userPaymentRequest.TokenPayment,
                payment_description = "Pago Bangk",
                customer_email = userPaymentRequest.Email,
                acceptance_token = merchantsInfo.Result.Data.Presigned_Acceptance.Acceptance_Token,
                accept_personal_auth = merchantsInfo.Result.Data.Presigned_Personal_Data_Auth.Acceptance_Token

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
                    var apiResponseBancolombia = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentWompiResponseDto>(jsonResponse);

                    userPaymentRequest.PaymentSourceId = apiResponseBancolombia.Data.Id;
                    var resultTransaction = await _createTransactionBancolombiaService.Execute(userPaymentRequest);

                    response = resultTransaction;
                    if (resultTransaction.HasErrors)
                    {
                        response = new ResponseDto<PaymentButtonBancolombiaResponseDto>
                        {
                            HasErrors = true,
                            Errors = new List<string> { resultTransaction.Errors.First() }
                        };
                    }
                }
                else
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    //Logs.WriteErrorLog($"Error: {errorMessage} - StatusCode: {apiResponse.StatusCode}");
                    response = new ResponseDto<PaymentButtonBancolombiaResponseDto>
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
