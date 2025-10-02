using Bangk.Common;
using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.Integrations;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Integrations
{
    public class GenerateTokenTrxBancolombiaService : IGenerateTokenTrxBancolombiaService
    {
        private readonly IConfiguration _configuration;
        private readonly IMedEnsuranceApplicationRepository _medInsuranceApplicationRepository;
        private readonly IPetInsuranceApplicationRepository _petInsuranceApplicationRepository;
        public GenerateTokenTrxBancolombiaService(IConfiguration configuration,
            IApplicationTransactionRepository applicationTransactionRepository,
            IMedEnsuranceApplicationRepository medInsuranceApplicationRepository,
            IPetInsuranceApplicationRepository petInsuranceApplicationRepository)
        {
            _configuration = configuration;
            _medInsuranceApplicationRepository = medInsuranceApplicationRepository;
            _petInsuranceApplicationRepository = petInsuranceApplicationRepository;
        }
        public async Task<ResponseDto<Bangk.Common.Dto.BancolombiaButtonTransferResponseDto.BancolombiaButtonTransferResponseDto>>
            Execute(int applicationId, bool isMed)
        {
            var response = new ResponseDto<Bangk.Common.Dto.BancolombiaButtonTransferResponseDto.BancolombiaButtonTransferResponseDto>();
            var wompiUrl = _configuration.GetSection("WompiKeys:UrlWompipayment").Value;
            var publicKey = _configuration.GetSection("WompiKeys:PublicKey").Value;

            wompiUrl = $"{wompiUrl}tokens/bancolombia_transfer";
            var query = new
            {
                redirect_url = _configuration.GetSection("WompiKeys:RedirectBankgUrl").Value + "&ApplicationId=" + applicationId + "&Type=" + (isMed ? "0" : "1"),
                type_auth = "TOKEN"
            };

            var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(query), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", publicKey);
                var apiResponse = await client.PostAsync(wompiUrl, jsonContent);
                //Logs.WriteInfoLog($"Status Code API: {apiResponse.StatusCode}");

                if (apiResponse.IsSuccessStatusCode)
                {
                    var jsonResponse = await apiResponse.Content.ReadAsStringAsync();
                    var apiResponseBancolombia = Newtonsoft.Json.JsonConvert.DeserializeObject<Bangk.Common.Dto.BancolombiaButtonTransferResponseDto.BancolombiaButtonTransferResponseDto>(jsonResponse);
                    //Logs.WriteInfoLog($"Response: {jsonResponse}");

                    if (isMed)
                    {
                        var application = await _medInsuranceApplicationRepository.GetById(applicationId);
                        application.BancolombiaTransactionId = apiResponseBancolombia.Data.Id;
                        await _medInsuranceApplicationRepository.Update(application);
                    }
                    else
                    {
                        var application = await _petInsuranceApplicationRepository.GetById(applicationId);
                        application.BancolombiaTransactionId = apiResponseBancolombia.Data.Id;
                        await _petInsuranceApplicationRepository.Update(application);
                    }

                    response = new ResponseDto<Common.Dto.BancolombiaButtonTransferResponseDto.BancolombiaButtonTransferResponseDto>
                    {
                        HasErrors = false,
                        Result = apiResponseBancolombia
                    };
                }
                else
                {
                    var errorMessage = await apiResponse.Content.ReadAsStringAsync();
                    //Logs.WriteErrorLog($"Error: {errorMessage} - StatusCode: {apiResponse.StatusCode}");
                    response = new ResponseDto<Common.Dto.BancolombiaButtonTransferResponseDto.BancolombiaButtonTransferResponseDto>
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
