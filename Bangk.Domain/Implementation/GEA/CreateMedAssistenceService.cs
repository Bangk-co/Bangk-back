using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.GEA;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.GEA
{
    public class CreateMedAssistenceService : ICreateMedAssistenceService
    {
        private readonly IConfiguration _configuration;
        IMedEnsuranceApplicationRepository _medEnsuranceApplicationRepository;
        private readonly IGetApplicationTokenGEAService _getApplicationTokenGEAService;
        public CreateMedAssistenceService(IConfiguration configuration,
            IMedEnsuranceApplicationRepository medEnsuranceApplicationRepository,
            IGetApplicationTokenGEAService getApplicationTokenGEAService)
        {
            _configuration = configuration;
            _medEnsuranceApplicationRepository = medEnsuranceApplicationRepository;
            _getApplicationTokenGEAService = getApplicationTokenGEAService;
        }

        public async Task<ResponseDto<bool>> Execute(string applicationReference)
        {
            var token = await _getApplicationTokenGEAService.Execute();
            var baseUrl = _configuration.GetSection("GeaKeys:GeaUrl").Value;
            var application = await _medEnsuranceApplicationRepository.GetMedAssistenceGEA(applicationReference);
            foreach (var item in application.MedPersons)
            {
                using (HttpClient client = new())
                {
                    var cuentaId = int.Parse(_configuration["GeaKeys:IdCuenta"]);
                    var planId = int.Parse(_configuration["GeaKeys:IdPlan"]);
                    var tipoCveAfiliado = item.DocumentType switch
                    {
                        76 => 1,
                        77 => 2,
                        _ => 3
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
                        cveafiliado = item.DocumentNumber,
                        tipo_cveafiliado = tipoCveAfiliado,
                        cuenta = cuentaId,
                        plan = planId,
                        fecha_inicio_vigencia = fechaInicio,
                        fecha_fin_vigencia = fechaFin.ToString("yyyy-MM-dd"),
                        estado = "ACTIVO",
                        identificacion_pagador = application.DocumentNumber,
                        nombres = application.Name,
                        email = application.Email,
                        telefono_personal = application.Phone,
                    };

                    var request = new ClientGEADto
                    {
                        clientes = new List<ClientDatosDto>
                        {
                            new ClientDatosDto
                            {
                                datos = gea 
                            }
                        }
                    };

                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Result}");
                    var json = JsonConvert.SerializeObject(request);
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
                }
            }

            return new ResponseDto<bool>
            {
                HasErrors = false,
                Result = true
            };
        }
    }
}
