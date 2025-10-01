using Bangk.Common.Dto;
using Bangk.Domain.Contract.Integrations;
using Bangk.Repositories.Repositories;
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
    public class PaymentSourcesBancolombiaTransferService : IPaymentSourcesBancolombiaTransferService
    {
        private readonly IConfiguration _configuration;
        private readonly IGetMerchantsTokenService _getMerchantsTokenService;
        private readonly ICreateTransactionBancolombiaService _createTransactionBancolombiaService;
        private readonly IMedEnsuranceApplicationRepository _medEnsuranceApplicationRepository;
        private readonly IPetInsuranceApplicationRepository _petInsuranceApplicationRepository;
        public PaymentSourcesBancolombiaTransferService(IConfiguration configuration,
                                                        IGetMerchantsTokenService getMerchantsTokenService,
                                                        ICreateTransactionBancolombiaService createTransactionBancolombiaService,
                                                        IMedEnsuranceApplicationRepository medEnsuranceApplicationRepository,
                                                        IPetInsuranceApplicationRepository petInsuranceApplicationRepository)
        {
            _configuration = configuration;
            _getMerchantsTokenService = getMerchantsTokenService;
            _createTransactionBancolombiaService = createTransactionBancolombiaService;
            _medEnsuranceApplicationRepository = medEnsuranceApplicationRepository;
            _petInsuranceApplicationRepository = petInsuranceApplicationRepository;
        }

        public async Task<ResponseDto<PaymentButtonBancolombiaResponseDto>> Execute(RequestUserPaymentDto userPaymentRequest)
        {
            var response = new ResponseDto<PaymentButtonBancolombiaResponseDto>();
            UserPaymentRequest userPayment = new UserPaymentRequest();
            var merchantsInfo = await _getMerchantsTokenService.Execute();

            var wompiUrl = _configuration.GetSection("WompiKeys:UrlWompipayment").Value;
            var privateKey = _configuration.GetSection("WompiKeys:PrivateKey").Value;

            wompiUrl = $"{wompiUrl}payment_sources";

            if (userPaymentRequest.TypeApplication.Equals("MED", StringComparison.OrdinalIgnoreCase))
            {
                var medApp = await _medEnsuranceApplicationRepository.GetMedAssistanceByApplicationId(userPaymentRequest.ApplicationId);
                userPayment.Email = medApp.Email.Trim();
                userPayment.Amount = medApp.Amount.Value;
            }
            else
            {
                var petApp = await _petInsuranceApplicationRepository.GetPetAssistanceByApplicationId(userPaymentRequest.ApplicationId);
                userPayment.Email = petApp.Email.Trim();
                userPayment.Amount = petApp.Amount.Value;
            }

            var query = new
            {
                type = "BANCOLOMBIA_TRANSFER",
                token = userPaymentRequest.TokenPayment, //transactionId
                payment_description = "Pago Bangk",
                customer_email = userPayment.Email.Trim(),
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

                    userPayment.PaymentSourceId = apiResponseBancolombia.Data.Id;
                    userPayment.TokenPayment = userPaymentRequest.TokenPayment;
                    userPayment.ApplicationId = userPaymentRequest.ApplicationId;
                    userPayment.TypeApplication = userPaymentRequest.TypeApplication;
                    
                    var resultTransaction = await _createTransactionBancolombiaService.Execute(userPayment);

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
