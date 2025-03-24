using Bangk.Common.Dto;
using Bangk.Domain.Contract.Integrations;
using Bangk.Domain.Contract.Integrations.Wompi;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Integrations.Wompi
{
    public class GetStatusTransactionService : IGetStatusTransactionService
    {
        private readonly IConfiguration _configuration;
        private readonly IGetMerchantsTokenService _getMerchantsTokenService;
        private readonly IGenerateIntegrityKeyService _generateIntegrityKeyService;
        private readonly IPetInsuranceApplicationRepository _petEnsuranceApplicationRepository;
        private readonly IApplicationTransactionRepository _applicationTransactionRepository;
        private readonly IMedEnsuranceApplicationRepository _medEnsuranceApplicationRepository;
        public GetStatusTransactionService(IConfiguration configuration, IGetMerchantsTokenService getMerchantsTokenService, 
            IGenerateIntegrityKeyService generateIntegrityKeyService,
            IPetInsuranceApplicationRepository petEnsuranceApplicationRepository,
            IApplicationTransactionRepository applicationTransactionRepository,
            IMedEnsuranceApplicationRepository medEnsuranceApplicationRepository)
        {
            _configuration = configuration;
            _getMerchantsTokenService = getMerchantsTokenService;
            _generateIntegrityKeyService = generateIntegrityKeyService;
            _petEnsuranceApplicationRepository = petEnsuranceApplicationRepository;
            _applicationTransactionRepository = applicationTransactionRepository;
            _medEnsuranceApplicationRepository = medEnsuranceApplicationRepository;
        }
        public async Task<ResponseDto<string>> Execute(string transactionReferences)
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
                    var medApplication = await _medEnsuranceApplicationRepository.GetMedEnsuranceApplicationByReference(reference);
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

                response.Result = apiResponseStatus.Data.Status;
            }

            return response;
        }
    }
}
