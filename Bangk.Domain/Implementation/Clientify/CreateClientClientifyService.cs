using Bangk.Common.Dto;
using Bangk.Domain.Contract.Clientify;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Clientify
{
    public class CreateClientClientifyService : ICreateClientClientifyService
    {
        private readonly ISettingRepository _settingRepository;
        private readonly IConfiguration _configuration;
        public CreateClientClientifyService(ISettingRepository settingRepository,
            IConfiguration configuration)
        {
            _settingRepository = settingRepository;
            _configuration = configuration;
        }

        public async Task<long> Execute(CreateClientDto createClientDto, string reviewerEmail)
        {
            var apiClient = new ClientifyService();
            // Autenticación y obtención del token
            await apiClient.LoginAsync("v1/api-auth/obtain_token/", _configuration.GetSection("ClientifyUser").Value,
                _configuration.GetSection("ClientifyPassword").Value);

            // se valida si el usuario existe
            var identification = createClientDto.IdentificationNumber;
            var email = createClientDto.Email;
            var resultadoGet = await apiClient.GetAsync($"/v1/contacts/?taxpayer_identification_number={identification}&email={email}");
            var responseData = JsonConvert.DeserializeObject<dynamic>(resultadoGet);
            long clientId;
            if (responseData.count > 0)
            {
                var results = responseData.results[0];
                clientId = (long)results["id"];
            }
            else
            {
                var codePhoneNumber = await _settingRepository.GetById(createClientDto.CountryCodePhoneId);
                var gender = await _settingRepository.GetById(createClientDto.GenderId);
                //se crea un nuevo cliente si no existe
                ClientClientifyDto clientClintifyDto = new()
                {
                    birthday = createClientDto.BirthDate.ToString("yyyy-MM-dd"),
                    email = email,
                    first_name = createClientDto.Name,
                    last_name = createClientDto.LastName,
                    gdpr_acceptance_date = createClientDto.PermissionTreatPersonalDataDate.ToString("yyyy-MM-ddTHH:mm:ss.ffffffK"),
                    phone = codePhoneNumber.Code + createClientDto.PhoneNumber,
                    taxpayer_identification_number = identification,
                    contact_source = "Bangk.co",
                    status = "in-deal",
                    owner = reviewerEmail,
                    custom_fields = new()
                    {
                        new CustomFieldsDto
                        {
                            field = "Género",
                            value = gender.Name
                        }
                    }
                };

                var response = await apiClient.PostAsync("/v1/contacts/", clientClintifyDto);
                dynamic jsonObject = JsonConvert.DeserializeObject<ExpandoObject>(response);
                clientId = jsonObject.id;
            }
            var bank = await _settingRepository.GetById(createClientDto.LoadAplicacionDto.BankId);
            var loanType = await _settingRepository.GetById(createClientDto.LoadAplicacionDto.LoanTypeId);

            //
            //crear o asociar deal
            DealClientifyDto dealClientifyDto = new()
            {
                amount = createClientDto.LoadAplicacionDto.LoanBalance,
                currency = "COP",
                contact = $"https://api.clientify.net/v1/contacts/{clientId}/",
                pipeline = "https://api.clientify.net/v1/deals/pipelines/45676/",
                expected_closed_date = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd"),
                name = $"Vida Deudor SBS_{createClientDto.Name} {createClientDto.LastName}",
                pipeline_stage_desc = "Validación",
                pipeline_desc = "Flujo comercial Endosos Seguros Vida",
                owner = reviewerEmail,
                contact_source = "Bangk.co",
                custom_fields = new()
                {
                    new CustomFieldsDto
                    {
                        field = "Producto",
                        value = "['Póliza Vida deudor SBS Seguros']"
                    },
                    new CustomFieldsDto
                    {
                        field = "Beneficiario Oneroso Seguro Vida Deudor",
                        value = $"['{bank.Name}']"
                    },
                    new CustomFieldsDto
                    {
                        field = "Créditos vigentes",
                        value = $"['{loanType.Name}']"
                    },
                    new CustomFieldsDto
                    {
                        field = "Valor asegurado para Seguro Vida Deudor",
                        value = createClientDto.LoadAplicacionDto.LoanBalance.ToString()
                    },
                    new CustomFieldsDto
                    {
                        field = "Número de crédito",
                        value = createClientDto.LoadAplicacionDto.LoanNumber
                    }
                }

            };

            var dealResponse = await apiClient.PostAsync("/v1/deals/", dealClientifyDto);
            dynamic dealObject = JsonConvert.DeserializeObject<ExpandoObject>(dealResponse);
            apiClient.Dispose();
            return dealObject.id;
        }
    }
}

