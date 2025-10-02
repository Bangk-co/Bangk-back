using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.GEA;
using Bangk.Domain.Contract.Integrations;
using Bangk.Domain.Contract.Integrations.Wompi;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Integrations.Wompi
{
    public class GetStatusTransactionService : IGetStatusTransactionService
    {
        private readonly IConfiguration _configuration;
        private readonly IPetInsuranceApplicationRepository _petEnsuranceApplicationRepository;
        private readonly IApplicationTransactionRepository _applicationTransactionRepository;
        private readonly IMedEnsuranceApplicationRepository _medEnsuranceApplicationRepository;
        private readonly ICreateMedAssistenceService _createMedAssistenceService;
        private readonly ICreatePetAssistenceService _createPetAssistenceService;
        public GetStatusTransactionService(IConfiguration configuration, IGetMerchantsTokenService getMerchantsTokenService, 
            IGenerateIntegrityKeyService generateIntegrityKeyService,
            IPetInsuranceApplicationRepository petEnsuranceApplicationRepository,
            IApplicationTransactionRepository applicationTransactionRepository,
            IMedEnsuranceApplicationRepository medEnsuranceApplicationRepository,
            ICreateMedAssistenceService createMedAssistenceService, 
            ICreatePetAssistenceService createPetAssistenceService)
        {
            _configuration = configuration;
            _petEnsuranceApplicationRepository = petEnsuranceApplicationRepository;
            _applicationTransactionRepository = applicationTransactionRepository;
            _medEnsuranceApplicationRepository = medEnsuranceApplicationRepository;
            _createMedAssistenceService = createMedAssistenceService;
            _createPetAssistenceService = createPetAssistenceService;
        }
        public async Task<ResponseDto<string>> Execute(string transactionReferences, bool updateStatus = true)
        {
            var response = new ResponseDto<string>();
            var baseUrl = _configuration.GetSection("WompiKeys:UrlWompipayment").Value;
            var publicKey = _configuration.GetSection("WompiKeys:PublicKey").Value;
            var transactionId = string.Empty;
            switch (transactionReferences)
            {
                case string reference when reference.Contains("PET"):
                    var petApplication = await _petEnsuranceApplicationRepository.GetPetApplicationByReference(reference);
                    transactionId = petApplication.TransactionId;
                    break;
                case string reference when reference.Contains("MED"):
                    var medApplication = await _medEnsuranceApplicationRepository.GetMedInsuranceApplicationByReference(reference);
                    transactionId = medApplication.TransactionId; 
                    break;
            }


            var getStatusUrl = $"{baseUrl}transactions/{transactionId}";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {publicKey}");
                HttpResponseMessage jsonResponse = await client.GetAsync(getStatusUrl);
                string result = await jsonResponse.Content.ReadAsStringAsync();
                var apiResponseStatus = JsonConvert.DeserializeObject<StatusTransactionResponseDto>(result);

                if (!jsonResponse.IsSuccessStatusCode)
                {
                    response.HasErrors = true;
                    response.Errors = new List<string> { $"Se ha generado un error procesando la solicitud" };
                    return response;
                }

                if (string.Equals(apiResponseStatus.Data.Status, "APPROVED"))
                {
                    await ApprovedTransactionProcess(apiResponseStatus.Data.Reference, apiResponseStatus.Data.Created_At);
                    await _applicationTransactionRepository.Create(CreateTransactionDto(apiResponseStatus));
                }
                else if (string.Equals(apiResponseStatus.Data.Status, "DECLINED") ||
                      string.Equals(apiResponseStatus.Data.Status, "VOIDED") ||
                      string.Equals(apiResponseStatus.Data.Status, "ERROR"))
                {
                    if (updateStatus)
                    {
                        await RejectedTransactionProcess(apiResponseStatus.Data.Reference);
                    }
                   
                    await _applicationTransactionRepository.Create(CreateTransactionDto(apiResponseStatus));
                }

                
                response.Result = apiResponseStatus.Data.Status;
            }

            return response;
        }

        private static ApplicationTransaction CreateTransactionDto(StatusTransactionResponseDto apiResponseStatus)
        {
            //Create log transaction
            return new ApplicationTransaction
            {
                Amount = apiResponseStatus.Data.Amount_In_Cents,
                Currency = apiResponseStatus.Data.Currency,
                Reference = apiResponseStatus.Data.Reference,
                Status = apiResponseStatus.Data.Status,
                TransactionCreationDate = apiResponseStatus.Data.Created_At,
                TransactionNumber = apiResponseStatus.Data.Id,
                PaymentMethodType = apiResponseStatus.Data.Payment_Method_Type,
                PaymentInstallments = apiResponseStatus.Data?.Payment_Method?.Installments,
            };
        }

        private async Task ApprovedTransactionProcess(string referenceApplication, DateTime CreatedTransactionDate)
        {
            switch (referenceApplication)
            {
                case string reference when reference.Contains("PET"):
                    var petApplication = await _petEnsuranceApplicationRepository.GetPetApplicationByReference(referenceApplication);
                    petApplication.Status = (int)InsuranceStatesEnum.Approve;
                    await _petEnsuranceApplicationRepository.Update(petApplication);
                    //Create Assistence in GEA
                    await _createPetAssistenceService.Execute(referenceApplication, CreatedTransactionDate);
                    break;
                case string reference when reference.Contains("MED"):
                    var medApplication = await _medEnsuranceApplicationRepository.GetMedInsuranceApplicationByReference(referenceApplication);
                    medApplication.Status = (int)InsuranceStatesEnum.Approve;
                    await _medEnsuranceApplicationRepository.Update(medApplication);
                    //Create Assistence in GEA
                    await _createMedAssistenceService.Execute(referenceApplication, CreatedTransactionDate);
                    break;
            }
        }


        private async Task RejectedTransactionProcess(string referenceApplication)
        {
            switch (referenceApplication)
            {
                case string reference when reference.Contains("PET"):
                    var petApplication = await _petEnsuranceApplicationRepository.GetPetApplicationByReference(reference);
                    petApplication.Status = (int)InsuranceStatesEnum.Rejected;
                    await _petEnsuranceApplicationRepository.Update(petApplication);
                    break;
                case string reference when reference.Contains("MED"):
                    var medApplication = await _medEnsuranceApplicationRepository.GetMedInsuranceApplicationByReference(reference);
                    medApplication.Status = (int)InsuranceStatesEnum.Rejected;
                    await _medEnsuranceApplicationRepository.Update(medApplication);
                    break;
            }
        }
    }
}
