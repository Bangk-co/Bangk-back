using Bangk.Common.Dto;
using Bangk.Common.Dto.MecharntsInfoDto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.GEA;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.GEA
{
    public class CreatePetAssistenceService : ICreatePetAssistenceService
    {
        private readonly IConfiguration _configuration;
        private readonly IPetInsuranceApplicationRepository _petInsuranceApplicationRepository;
        private readonly IGetApplicationTokenGEAService _getApplicationTokenGEAService;
        public CreatePetAssistenceService(IConfiguration configuration,
            IPetInsuranceApplicationRepository petInsuranceApplicationRepository,
            IGetApplicationTokenGEAService getApplicationTokenGEAService)
        {
            _configuration = configuration;
            _petInsuranceApplicationRepository = petInsuranceApplicationRepository;
            _getApplicationTokenGEAService = getApplicationTokenGEAService;
        }

        public async Task<ResponseDto<bool>> Execute(string applicationReference)
        {
            var token = await _getApplicationTokenGEAService.Execute();
            var baseUrl = _configuration.GetSection("GeaKeys:GeaUrl").Value;
            var application = await _petInsuranceApplicationRepository.GetPetAssistenceGEA(applicationReference);
            var count = 1;
            foreach (var item in application.Pets)
            {
                using (HttpClient client = new HttpClient())
                {
                    var clients = new ClientGEAPetDto();
                    var clientPets = new List<DatosWrapperDto>();

                    DatosWrapperDto datos = new DatosWrapperDto();
                    var cuentaId = int.Parse(_configuration["GeaKeys:IdCuenta"]);
                    var planId = int.Parse(_configuration["GeaKeys:IdPlan"]);
                    var tipoCveAfiliado = application.DocumentType switch
                    {
                        (int)DocumenIdentificationTypeEnum.Cc => 1,
                        (int)DocumenIdentificationTypeEnum.Ce=> 2,
                        _ => 3
                    };
                    var petType = item.PetType switch
                    {
                        (int)PetType.Cat => "Gato",
                        _ => "Perro"
                    };

                    var fechaInicio = application.CreatedDate.AddDays(1).ToString("yyyy-MM-dd");
                    var fechaFin = application.PaymentOcurrencyId switch
                    {
                        (int)MethodPaymentTimeEnum.Mensual => application.CreatedDate.AddMonths(1),
                        (int)MethodPaymentTimeEnum.Semestral => application.CreatedDate.AddMonths(6),
                        _ => application.CreatedDate.AddYears(1)
                    };

                    var gea = new DataGEADto
                    {
                        cveafiliado = $"{application.DocumentNumber}-{count}",
                        tipo_cveafiliado = tipoCveAfiliado,
                        cuenta = cuentaId,
                        plan = planId,
                        fecha_inicio_vigencia = fechaInicio,
                        fecha_fin_vigencia = fechaFin.ToString("yyyy-MM-dd"),
                        estado = "ACTIVO",
                        identificacion_pagador = application.DocumentNumber,
                        nombres = application.Name,
                        email = application.Email.Trim(),
                        telefono_personal = application.Phone,
                    };

                    var datosExtra = new DataGEAExtrasDto
                    {
                        color_mascota = item.Color,
                        cuenta = cuentaId,
                        cveafiliado = $"{application.DocumentNumber}-{count}",
                        edad_mascota = item.Age,
                        nombre_mascota = item.Name,
                        raza_mascota = item.Breed,
                        tipo_mascota = petType,
                    };

                    datos.datos = gea;
                    datos.datosExtras = datosExtra;
                    clientPets.Add(datos);
                    clients.clientes = clientPets;
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Result}");
                    var json = JsonConvert.SerializeObject(clients);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage jsonResponse = await client.PostAsync(baseUrl + "v1/afiliacion/now", content);
                    string result = await jsonResponse.Content.ReadAsStringAsync();
                    var apiResponseCard = JsonConvert.DeserializeObject<CardTokenResponseDto>(result);

                    if (!jsonResponse.IsSuccessStatusCode)
                    {
                        return new ResponseDto<bool>
                        {
                            Result = false,
                            HasErrors = true,
                            Errors = new List<string> { $"Se ha generado un error creando la asistencia en GEA" }
                        };
                    }

                    count++;
                }
            }


            return new ResponseDto<bool>();
        }
    }
}
