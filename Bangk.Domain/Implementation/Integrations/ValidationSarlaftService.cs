using Bangk.Common.Dto;
using Bangk.Domain.Contract.Integrations;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Integrations
{
    public class ValidationSarlaftService : IValidationSarlaftService
    {
        private readonly ILoanInsurancePolicyRepository _loanInsurancePolicyRepository;
        private readonly IConfiguration _configuration;
        private readonly IClientRepository _clientRepository;
        public ValidationSarlaftService(ILoanInsurancePolicyRepository loanInsurancePolicyRepository, IConfiguration configuration, IClientRepository clientRepository)
        {
            _loanInsurancePolicyRepository = loanInsurancePolicyRepository;
            _configuration = configuration;
            _clientRepository = clientRepository;
        }

        public async Task Execute(int loanApplicationId)
        {
            var client = await _clientRepository.GetClientByLoanApplicationId(loanApplicationId);
            var clientSarlaftInf = await _loanInsurancePolicyRepository.GetClientSarlaftInfoDtoyByLoanApplicationId(loanApplicationId);

            string apiUrl = _configuration.GetSection("Sarlaft:Url").Value;
            var parameter = new SarlaftParametersDto
            {
                DigitoVerificacion = string.Empty,
                EsPPE = clientSarlaftInf.IsPublicPerson,
                FechaExpedicion = client.DocumentIssueDate.HasValue ? client.DocumentIssueDate.Value.ToString("yyyyMMdd") : string.Empty,
                NumeroCelular = client.PhoneNumber,
                NumeroDocumento = client.IdentificationNumber,
                PrimaKMs = clientSarlaftInf.YearlyFee,
                TipoConsulta = "1",
                TipoDocumento = client.DocumenIdentificationTypeId == 76 ? "1" : "2",
                TipoParticipante = string.Empty,
                ValorAsegurado = clientSarlaftInf.LoanBalance,
                VinculoLaboral = string.Empty,
                VinculosPPE = clientSarlaftInf.HasBondPublicPerson,
                Intermediario = _configuration.GetSection("Sarlaft:Intermediario").Value,
                Ramo = string.Empty,
                Sponsor = string.Empty

            };

            string jsonBody = JsonConvert.SerializeObject(parameter);
            var byteArray = new UTF8Encoding().GetBytes($"{_configuration.GetSection("Sarlaft:User").Value}:{_configuration.GetSection("Sarlaft:Password").Value}");

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);

                }
                else
                {
                    Console.WriteLine($"Error al llamar a la API. Código de estado: {response.StatusCode}");
                }
            }
        }
    }
}
