using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.Integrations;
using Bangk.Domain.Contract.Integrations.Wompi;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
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
        private readonly IPetInsuranceApplicationRepository _petEnsuranceApplicationRepository;
        private readonly IApplicationTransactionRepository _applicationTransactionRepository;
        public CardPaymentService(IConfiguration configuration, IGetMerchantsTokenService getMerchantsTokenService, 
            IGenerateIntegrityKeyService generateIntegrityKeyService,
            IPetInsuranceApplicationRepository petEnsuranceApplicationRepository,
            IApplicationTransactionRepository applicationTransactionRepository)
        {
            _configuration = configuration;
            _getMerchantsTokenService = getMerchantsTokenService;
            _generateIntegrityKeyService = generateIntegrityKeyService;
            _petEnsuranceApplicationRepository = petEnsuranceApplicationRepository;
            _applicationTransactionRepository = applicationTransactionRepository;
        }
        public async Task<ResponseDto<PaymentCardResponseDto>> Execute(CardPaymentDto cardPaymentDto)
        {
            var response = new ResponseDto<PaymentCardResponseDto>();
            var baseUrl = _configuration.GetSection("WompiKeys:UrlWompipayment").Value;
            var publicKey = _configuration.GetSection("WompiKeys:PublicKey").Value;
            var privateKey = _configuration.GetSection("WompiKeys:PrivateKey").Value;
            var wompiUrl = $"{baseUrl}tokens/cards";
            var paymentSourceUrl = $"{baseUrl}payment_sources";
            var transactionUrl = $"{baseUrl}transactions";
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

                if (!jsonResponse.IsSuccessStatusCode || apiResponseCard.Status != "CREATED")
                {
                    response = new ResponseDto<PaymentCardResponseDto>
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
                    acceptance_token,
                    accept_personal_auth,
                };

                json = JsonConvert.SerializeObject(paymentSource);
                client.DefaultRequestHeaders.Remove("Authorization");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {privateKey}");
                content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage responsePaymentSource = await client.PostAsync(paymentSourceUrl, content);
                result = await responsePaymentSource.Content.ReadAsStringAsync();
                var apiResponsePaymentSource = JsonConvert.DeserializeObject<CardPaymentSourceDto>(result);

                if (!responsePaymentSource.IsSuccessStatusCode || apiResponsePaymentSource.Data.Status != "AVAILABLE")
                {
                    response = new ResponseDto<PaymentCardResponseDto>
                    {
                        HasErrors = true,
                        Errors = new List<string> { $"Se ha generado un error procesando la solicitud" }
                    };
                }

                //Transaction
                var reference = $"{cardPaymentDto.TypeApplication}-{Guid.NewGuid().ToString("N")}";
                var signature = await _generateIntegrityKeyService.Execute(cardPaymentDto.Amount, reference);
                var transaction = new
                {
                    amount_in_cents = cardPaymentDto.Amount,
                    currency = "COP",
                    signature = signature,
                    customer_email = cardPaymentDto.Email,
                    payment_method = new
                    {
                        installments = cardPaymentDto.PaymentInstallments
                    },
                    reference,
                    payment_source_id = apiResponsePaymentSource.Data.Id
                };

                json = JsonConvert.SerializeObject(transaction);
                content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage responseTransaction = await client.PostAsync(transactionUrl, content);
                result = await responseTransaction.Content.ReadAsStringAsync();
                var apiResponseTransaction = JsonConvert.DeserializeObject<dynamic>(result);
                if (!responseTransaction.IsSuccessStatusCode)
                {
                    response = new ResponseDto<PaymentCardResponseDto>
                    {
                        HasErrors = true,
                        Errors = new List<string> { $"Se ha generado un error procesando la solicitud" }
                    };
                }

                //Save tokenization
                var petApplication = await _petEnsuranceApplicationRepository.GetById(cardPaymentDto.PetApplicationId);
                petApplication.PaymentSourceId = apiResponsePaymentSource.Data.Id;
                petApplication.TransactionReference = reference;
                await _petEnsuranceApplicationRepository.Update(petApplication);

                //Create log transaction
                var transactionAdd = new ApplicationTransaction
                {
                    Amount = cardPaymentDto.Amount,
                    Currency = apiResponseTransaction["data"]["currency"],
                    Reference = apiResponseTransaction["data"]["reference"],
                    Status = apiResponseTransaction["data"]["status"],
                    TransactionCreationDate = apiResponseTransaction["data"]["created_at"],
                    TransactionNumber = apiResponseTransaction["data"]["id"],
                    PaymentMethodType = apiResponseTransaction["data"]["payment_method_type"],
                };

                await _applicationTransactionRepository.Create(transactionAdd);
                response.Result = new PaymentCardResponseDto
                {
                    Amount = cardPaymentDto.Amount,
                    Status = transactionAdd.Status,
                    TransactionReference = transactionAdd.Reference
                };
            }

            return response;
        }
    }
}
