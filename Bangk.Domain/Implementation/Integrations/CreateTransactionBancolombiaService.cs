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
using Bangk.Repositories.Repositories;
using Bangk.Repositories.Entities;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace Bangk.Domain.Implementation.Integrations
{
    public class CreateTransactionBancolombiaService : ICreateTransactionBancolombiaService
    {
        private readonly IConfiguration _configuration;
        private readonly IGenerateIntegrityKeyService _generateIntegrityKeyService;
        private readonly IApplicationTransactionRepository _applicationTransactionRepository;
        private readonly IGetTransactionPaymentStatusService _getTransactionPaymentStatusService;
        private readonly IPetInsuranceApplicationRepository _petEnsuranceApplicationRepository;
        private readonly IMedEnsuranceApplicationRepository _medEnsuranceApplicationRepository;
        public CreateTransactionBancolombiaService(IConfiguration configuration,
                                                   IGenerateIntegrityKeyService generateIntegrityKeyService,
                                                   IApplicationTransactionRepository applicationTransactionRepository,
                                                   IGetTransactionPaymentStatusService getTransactionPaymentStatusServi,
                                                   IMedEnsuranceApplicationRepository medEnsuranceApplicationRepository,
                                                   IPetInsuranceApplicationRepository petEnsuranceApplicationRepository)
        {
            _configuration = configuration;
            _generateIntegrityKeyService = generateIntegrityKeyService;
            _applicationTransactionRepository = applicationTransactionRepository;
            _getTransactionPaymentStatusService = getTransactionPaymentStatusServi;
            _medEnsuranceApplicationRepository = medEnsuranceApplicationRepository;
            _petEnsuranceApplicationRepository = petEnsuranceApplicationRepository;
        }

        public async Task<ResponseDto<PaymentButtonBancolombiaResponseDto>> Execute(UserPaymentRequest userPaymentRequest)
        {
            var response = new ResponseDto<PaymentButtonBancolombiaResponseDto>();
            var currency = "COP";
            var wompiUrl = _configuration.GetSection("WompiKeys:UrlWompipayment").Value;
            var privateKey = _configuration.GetSection("WompiKeys:PrivateKey").Value;
            var reference = $"{userPaymentRequest.TypeApplication}-{Guid.NewGuid().ToString("N")}";
            var integrityKey = await _generateIntegrityKeyService.Execute(userPaymentRequest.Amount, reference);

            wompiUrl = $"{wompiUrl}transactions";
            var query = new
            {
                amount_in_cents = userPaymentRequest.Amount, 
                currency = currency, 
                signature = integrityKey, 
                customer_email = userPaymentRequest.Email, 
                reference = reference, 
                payment_source_id = userPaymentRequest.PaymentSourceId,
            };

            var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(query), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", privateKey);
                HttpResponseMessage apiResponse = await client.PostAsync(wompiUrl, jsonContent);
                //Logs.WriteInfoLog($"Status Code API: {apiResponse.StatusCode}");
                if (apiResponse.IsSuccessStatusCode)
                {

                    var jsonResponse = await apiResponse.Content.ReadAsStringAsync();
                    var apiResponseTransaction = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    var transactionFinalId = apiResponseTransaction["data"]["id"].ToString();
                    var transactionStatus = await _getTransactionPaymentStatusService.Execute(transactionFinalId);
                    //Create log transaction

                    var transactionAdd = new ApplicationTransaction
                    {
                        Amount = userPaymentRequest.Amount,
                        Currency = apiResponseTransaction["data"]["currency"],
                        Reference = apiResponseTransaction["data"]["reference"],
                        Status = transactionStatus.Result["data"]["status"].ToString(),
                        TransactionCreationDate = apiResponseTransaction["data"]["created_at"],
                        TransactionNumber = apiResponseTransaction["data"]["id"],
                        PaymentMethodType = apiResponseTransaction["data"]["payment_method_type"],
                    };

                    if (userPaymentRequest.TypeApplication.Equals("MED", StringComparison.OrdinalIgnoreCase))
                    {
                        var medApplicationInBD = await _medEnsuranceApplicationRepository.GetById(userPaymentRequest.ApplicationId);
                        medApplicationInBD.TokenPaymentSource = userPaymentRequest.PaymentSourceId;
                        medApplicationInBD.TransactionReference = reference;
                        medApplicationInBD.TransactionId = apiResponseTransaction["data"]["id"];
                        medApplicationInBD.Amount = userPaymentRequest.Amount;
                        await _medEnsuranceApplicationRepository.Update(medApplicationInBD);
                    }
                    else if (userPaymentRequest.TypeApplication.Equals("PET", StringComparison.OrdinalIgnoreCase))
                    {
                        var petApplicationInBD = await _petEnsuranceApplicationRepository.GetById(userPaymentRequest.ApplicationId);
                        petApplicationInBD.TokenPaymentSource = userPaymentRequest.PaymentSourceId;
                        petApplicationInBD.TransactionReference = reference;
                        petApplicationInBD.TransactionId = apiResponseTransaction["data"]["id"];
                        petApplicationInBD.Amount = userPaymentRequest.Amount;
                        await _petEnsuranceApplicationRepository.Update(petApplicationInBD);
                    }

                    await _applicationTransactionRepository.Create(transactionAdd);

                    var menssage = new PaymentButtonBancolombiaResponseDto
                    {
                        TransactionReference = reference,
                        TransactionId = apiResponseTransaction["data"]["id"],
                        Amount = userPaymentRequest.Amount / 100
                    };

                    response = new ResponseDto<PaymentButtonBancolombiaResponseDto>
                    {
                        HasErrors = false,
                        Result = menssage
                    };
                }
                else
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    response = new ResponseDto<PaymentButtonBancolombiaResponseDto>
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
