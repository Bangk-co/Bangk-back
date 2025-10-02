using Bangk.Common.Dto;
using Bangk.Common.Enums;
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
    public class PaymentAssistanceJobService : IPaymentAssistanceJobService
    {
        private readonly IPetInsuranceApplicationRepository _petInsuranceApplicationRepository;
        private readonly IMedEnsuranceApplicationRepository _medEnsuranceApplicationRepository;
        private readonly IGenerateIntegrityKeyService _generateIntegrityKeyService;
        private readonly IGetStatusTransactionService _getStatusTransactionService;
        private readonly IApplicationTransactionRepository _applicationTransactionRepository;
        private readonly IConfiguration _configuration;
        private readonly ITryPaymentJobRepository _tryPaymentJobRepository;
        public PaymentAssistanceJobService(IPetInsuranceApplicationRepository petInsuranceApplicationRepository,
            IMedEnsuranceApplicationRepository medEnsuranceApplicationRepository,
            IConfiguration configuration,
            IGenerateIntegrityKeyService generateIntegrityKeyService,
            IGetStatusTransactionService getStatusTransactionService,
            IApplicationTransactionRepository applicationTransactionRepository,
            ITryPaymentJobRepository tryPaymentJobRepository)
        {
            _petInsuranceApplicationRepository = petInsuranceApplicationRepository;
            _medEnsuranceApplicationRepository = medEnsuranceApplicationRepository;
            _configuration = configuration;
            _generateIntegrityKeyService = generateIntegrityKeyService;
            _getStatusTransactionService = getStatusTransactionService;
            _applicationTransactionRepository = applicationTransactionRepository;
            _tryPaymentJobRepository = tryPaymentJobRepository;
        }
        public async Task Execute()
        {
            var baseUrl = _configuration.GetSection("WompiKeys:UrlWompipayment").Value;
            var privateKey = _configuration.GetSection("WompiKeys:PrivateKey").Value;
            var transactionUrl = $"{baseUrl}transactions";

            var petPendingToPay = await _petInsuranceApplicationRepository.GetPetAssistencePendingToPay(DateTime.UtcNow);
            await PetPaymentAssistence(petPendingToPay, privateKey, transactionUrl);

            var medPendingToPay = await _medEnsuranceApplicationRepository.GetMedAssistancePendingToPay(DateTime.UtcNow);
            await MedPaymentAssistence(medPendingToPay, privateKey, transactionUrl);
        }

        private async Task PetPaymentAssistence(List<PaymentJobDto> petPendingToPay, string privateKey, string transactionUrl)
        {
            foreach (var item in petPendingToPay)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {privateKey}");
                    var reference = $"PET-{Guid.NewGuid():N}";
                    var signature = await _generateIntegrityKeyService.Execute((item.Amount.Value), reference);
                    var transaction = new
                    {
                        amount_in_cents = item.Amount,
                        currency = "COP",
                        signature,
                        customer_email = item.Email.Trim(),
                        payment_method = new
                        {
                            installments = item.PaymentInstallments
                        },
                        reference,
                        payment_source_id = item.TokenPaymentSource
                    };

                    var json = JsonConvert.SerializeObject(transaction);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseTransaction = await client.PostAsync(transactionUrl, content);
                    var result = await responseTransaction.Content.ReadAsStringAsync();
                    var apiResponseTransaction = JsonConvert.DeserializeObject<dynamic>(result);
                    if (responseTransaction.IsSuccessStatusCode)
                    {
                        var transactionAdd = new ApplicationTransaction
                        {
                            Amount = item.Amount.Value,
                            Currency = apiResponseTransaction["data"]["currency"],
                            Reference = apiResponseTransaction["data"]["reference"],
                            Status = apiResponseTransaction["data"]["status"],
                            TransactionCreationDate = apiResponseTransaction["data"]["created_at"],
                            TransactionNumber = apiResponseTransaction["data"]["id"],
                            PaymentMethodType = apiResponseTransaction["data"]["payment_method_type"],
                            PaymentInstallments = item.PaymentInstallments
                        };

                        await _applicationTransactionRepository.Create(transactionAdd);

                        var petApp = await _petInsuranceApplicationRepository.GetById(item.ApplicationId);
                        petApp.TransactionReference = reference;
                        petApp.TransactionId = apiResponseTransaction["data"]["id"];
                        await _petInsuranceApplicationRepository.Update(petApp);

                        string status;
                        do
                        {
                            var statusApi = await _getStatusTransactionService.Execute(reference, false);
                            status = statusApi.Result;
                        }
                        while (string.Equals(status, "PENDING", StringComparison.OrdinalIgnoreCase));

                        if (string.Equals(status, "APPROVED"))
                        {
                            var nextPaymentDate = petApp.PaymentOcurrencyId switch
                            {
                                (int)MethodPaymentTimeEnum.Mensual => petApp.NextPaymentDate.Value.AddMonths(1),
                                (int)MethodPaymentTimeEnum.Semestral => petApp.NextPaymentDate.Value.AddMonths(6),
                                _ => petApp.NextPaymentDate.Value.AddYears(1)
                            };

                            petApp.NextPaymentDate = nextPaymentDate;
                            await _petInsuranceApplicationRepository.Update(petApp);
                        }
                        else if (string.Equals(status, "DECLINED") ||
                              string.Equals(status, "VOIDED") ||
                              string.Equals(status, "ERROR"))
                        {
                            await CreateNextTryPayment(reference);
                        }
                    }

                }
            }
        }

        private async Task MedPaymentAssistence(List<PaymentJobDto> medPendingToPay, string privateKey, string transactionUrl)
        {
            foreach (var item in medPendingToPay)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {privateKey}");
                    var reference = $"MED-{Guid.NewGuid():N}";
                    var signature = await _generateIntegrityKeyService.Execute((item.Amount.Value), reference);
                    var transaction = new
                    {
                        amount_in_cents = item.Amount,
                        currency = "COP",
                        signature,
                        customer_email = item.Email.Trim(),
                        payment_method = new
                        {
                            installments = item.PaymentInstallments
                        },
                        reference,
                        payment_source_id = item.TokenPaymentSource
                    };

                    var json = JsonConvert.SerializeObject(transaction);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseTransaction = await client.PostAsync(transactionUrl, content);
                    var result = await responseTransaction.Content.ReadAsStringAsync();
                    var apiResponseTransaction = JsonConvert.DeserializeObject<dynamic>(result);
                    if (responseTransaction.IsSuccessStatusCode)
                    {
                        var transactionAdd = new ApplicationTransaction
                        {
                            Amount = item.Amount.Value,
                            Currency = apiResponseTransaction["data"]["currency"],
                            Reference = apiResponseTransaction["data"]["reference"],
                            Status = apiResponseTransaction["data"]["status"],
                            TransactionCreationDate = apiResponseTransaction["data"]["created_at"],
                            TransactionNumber = apiResponseTransaction["data"]["id"],
                            PaymentMethodType = apiResponseTransaction["data"]["payment_method_type"],
                            PaymentInstallments = item.PaymentInstallments
                        };

                        await _applicationTransactionRepository.Create(transactionAdd);

                        var medApp = await _medEnsuranceApplicationRepository.GetById(item.ApplicationId);
                        medApp.TransactionReference = reference;
                        medApp.TransactionId = apiResponseTransaction["data"]["id"];
                        await _medEnsuranceApplicationRepository.Update(medApp);

                        string status;
                        do
                        {
                            var statusApi = await _getStatusTransactionService.Execute(reference, false);
                            status = statusApi.Result;
                        }
                        while (string.Equals(status, "PENDING", StringComparison.OrdinalIgnoreCase));

                        if (string.Equals(status, "APPROVED"))
                        {
                            var nextPaymentDate = medApp.PaymentOcurrencyId switch
                            {
                                (int)MethodPaymentTimeEnum.Mensual => medApp.NextPaymentDate.Value.AddMonths(1),
                                (int)MethodPaymentTimeEnum.Semestral => medApp.NextPaymentDate.Value.AddMonths(6),
                                _ => medApp.NextPaymentDate.Value.AddYears(1)
                            };

                            medApp.NextPaymentDate = nextPaymentDate;
                            await _medEnsuranceApplicationRepository.Update(medApp);
                        }
                        else if (string.Equals(status, "DECLINED") ||
                              string.Equals(status, "VOIDED") ||
                              string.Equals(status, "ERROR"))
                        {
                            await CreateNextTryPayment(reference);
                        }
                    }
                }
            }
        }

        public async Task CreateNextTryPayment(string reference)
        {
            var nextTry = new TryPaymentJob
            {
                ApplicationReference = reference,
                CountFailed = 1,
            };

            await _tryPaymentJobRepository.Create(nextTry);
        }
    }
}
