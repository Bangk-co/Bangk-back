using Bangk.Business.Extension.SenderGrid;
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
    public class PaymentAssistanceTryService : IPaymentAssistanceTryService
    {
        private readonly ITryPaymentJobRepository _tryPaymentJobRepository;
        private readonly IPetInsuranceApplicationRepository _petInsuranceApplicationRepository;
        private readonly IMedEnsuranceApplicationRepository _medEnsuranceApplicationRepository;
        private readonly IConfiguration _configuration;
        private readonly IGenerateIntegrityKeyService _generateIntegrityKeyService;
        private readonly IApplicationTransactionRepository _applicationTransactionRepository;
        private readonly IGetStatusTransactionService _getStatusTransactionService;


        public PaymentAssistanceTryService(ITryPaymentJobRepository tryPaymentJobRepository,
            IConfiguration configuration,
            IGenerateIntegrityKeyService generateIntegrityKeyService,
            IApplicationTransactionRepository applicationTransactionRepository,
            IPetInsuranceApplicationRepository petInsuranceApplicationRepository,
            IMedEnsuranceApplicationRepository medEnsuranceApplicationRepository,
            IGetStatusTransactionService getStatusTransactionService)
        {
            _tryPaymentJobRepository = tryPaymentJobRepository;
            _configuration = configuration;
            _generateIntegrityKeyService = generateIntegrityKeyService;
            _applicationTransactionRepository = applicationTransactionRepository;
            _petInsuranceApplicationRepository = petInsuranceApplicationRepository;
            _medEnsuranceApplicationRepository = medEnsuranceApplicationRepository;
            _getStatusTransactionService = getStatusTransactionService;
        }

        public async Task Execute(int countFailed)
        {
            var baseUrl = _configuration.GetSection("WompiKeys:UrlWompipayment").Value;
            var privateKey = _configuration.GetSection("WompiKeys:PrivateKey").Value;
            var transactionUrl = $"{baseUrl}transactions";

            var aplicationPending = await _tryPaymentJobRepository.GetryPaymentJobByCountFail(countFailed);
            foreach (var item in aplicationPending)
            {
                switch (item.ApplicationReference)
                {
                    case string reference when reference.Contains("PET"):
                        var petApplication = await _petInsuranceApplicationRepository.GetPetAssistancePayment(reference);
                        await PetPaymentAssistence(petApplication, privateKey, transactionUrl, countFailed);
                        break;
                    case string reference when reference.Contains("MED"):
                        var medApplication = await _medEnsuranceApplicationRepository.GetMedAssistencePayment(reference);
                        await MedPaymentAssistence(medApplication, privateKey, transactionUrl, countFailed);
                        break;
                }
            }
        }

        private async Task PetPaymentAssistence(List<PaymentJobDto> petPendingToPay, string privateKey, string transactionUrl, int countFailed)
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
                            await DeleteNeNextTryPayment(item.TransactionReference);
                        }
                        else if (string.Equals(status, "DECLINED") ||
                              string.Equals(status, "VOIDED") ||
                              string.Equals(status, "ERROR"))
                        {
                            if (countFailed != 3)
                            {
                                await CreateNextTryPayment(item.TransactionReference, reference);
                            }
                            else
                            {
                                var templateId = _configuration.GetSection("SendGridEmailSender:CancelSuscriptionPets").Value;
                                await SendNotification(item.Email, templateId, item.ClientName, item.PaymentOcurrency);
                                await DeleteNeNextTryPayment(item.TransactionReference);
                                petApp.Status = (int)InsuranceStatesEnum.Rejected;
                                await _petInsuranceApplicationRepository.Update(petApp);
                            }
                        }
                    }
                }
            }
        }

        private async Task MedPaymentAssistence(List<PaymentJobDto> medPendingToPay, string privateKey, string transactionUrl, int countFailed)
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

                            await DeleteNeNextTryPayment(item.TransactionReference);
                        }
                        else if (string.Equals(status, "DECLINED") ||
                              string.Equals(status, "VOIDED") ||
                              string.Equals(status, "ERROR"))
                        {
                            if(countFailed != 3)
                            {
                                await CreateNextTryPayment(item.TransactionReference, reference);
                            }
                            else
                            {
                                var templateId = _configuration.GetSection("SendGridEmailSender:CancelSuscriptionTelemedicina").Value;
                                await SendNotification(item.Email, templateId, item.ClientName, item.PaymentOcurrency);
                                await DeleteNeNextTryPayment(item.TransactionReference);
                                medApp.Status = (int)InsuranceStatesEnum.Rejected;
                                await _medEnsuranceApplicationRepository.Update(medApp);
                            }
                        }
                    }
                }
            }
        }

        public async Task CreateNextTryPayment(string reference, string newReference)
        {
            var entity = await _tryPaymentJobRepository.GetryPaymentJobByReference(reference);
            entity.CountFailed += 1;
            entity.ApplicationReference = newReference;
            await _tryPaymentJobRepository.Update(entity);
        }

        public async Task DeleteNeNextTryPayment(string reference)
        {
            var entity = await _tryPaymentJobRepository.GetryPaymentJobByReference(reference);
            await _tryPaymentJobRepository.Delete(entity);
        }

        private async Task SendNotification(string email, string templateId, string clientName, int PaymentOcurrency)
        {
            var url = _configuration.GetSection("LoanApplicationAssigned:UrlFront").Value;
            var nextPayment = PaymentOcurrency switch
            {
                (int)MethodPaymentTimeEnum.Mensual => "Mensual",
                (int)MethodPaymentTimeEnum.Semestral => "Semestral",
                _ => "Anual"
            };
            var sendEmail = new SendGridEmailDto
            {
                Email = email,
                From = _configuration.GetSection("SendGridEmailSender:From").Value,
                Key = _configuration.GetSection("SendGridEmailSender:Key").Value,
                Name = _configuration.GetSection("SendGridEmailSender:Name").Value,
                TemplateId = templateId,

                Data = new
                {
                    clientname = clientName,
                    paymentocurrency = nextPayment,
                }
            };

            await SendGridEmailSender.SendEmailAsync(sendEmail);
        }
    }
}
