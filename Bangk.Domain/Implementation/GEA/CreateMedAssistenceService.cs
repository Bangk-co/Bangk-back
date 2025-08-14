using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.GEA;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<ResponseDto<bool>> Execute(string applicationReference, DateTime CreatedTransactionDate)
        {
            var token = await _getApplicationTokenGEAService.Execute();
            var baseUrl = _configuration.GetSection("GeaKeys:GeaUrl").Value;
            var cuentaId = int.Parse(_configuration["GeaKeys:IdCuenta"]);
            var planId = int.Parse(_configuration["GeaKeys:IdPlanTelemedicina"]);
            var application = await _medEnsuranceApplicationRepository.GetMedAssistenceGEA(applicationReference);
            var datosExtras = new DataGEAExtrasMedDto
            {
                identificacion_pagador = application.DocumentNumber,
                nombre_pagador = application.Name,
            };

            foreach (var item in application.MedPersons)
            {
                using (HttpClient client = new())
                {
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
                        tipo_cveafiliado = 1,
                        cuenta = cuentaId,
                        plan = planId,
                        fecha_inicio_vigencia = fechaInicio,
                        fecha_fin_vigencia = fechaFin.ToString("yyyy-MM-dd"),
                        estado = "ACTIVO",
                        identificacion = item.DocumentNumber,
                        tipo_identificacion = tipoCveAfiliado,
                        nombres = item.Name,
                        email = item.Email,
                        telefono_personal = item.PhoneNumber,
                    };

                    var request = new ClientGEADto
                    {
                        clientes = new List<ClientDatosDto>
                        {
                            new ClientDatosDto
                            {
                                datos = gea,
                                datos_extras = datosExtras
                            }
                        }
                    };

                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Result}");
                    var json = JsonConvert.SerializeObject(request);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage jsonResponse = await client.PostAsync(baseUrl + "v1/afiliacion/now", content);
                    string result = await jsonResponse.Content.ReadAsStringAsync();
                    if (!jsonResponse.IsSuccessStatusCode)
                    {
                        await SendEmailFailedCreationGEA(application, CreatedTransactionDate);
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

        private  async Task SendEmailFailedCreationGEA(CreateMedAssistenceGEADto application, DateTime CreatedTransactionDate)
        {
            
            var url = _configuration.GetSection("LoanApplicationAssigned:UrlFront").Value;
            var sendEmail = new SendGridEmailDto
            {
                Email = _configuration.GetSection("GeaKeys:EmailErrorGea").Value,
                Cc = _configuration.GetSection("GeaKeys:EmailCopyErrorGea").Value,
                From = _configuration.GetSection("SendGridEmailSender:From").Value,
                Key = _configuration.GetSection("SendGridEmailSender:Key").Value,
                Name = _configuration.GetSection("SendGridEmailSender:Name").Value,
                TemplateId = _configuration.GetSection("SendGridEmailSender:GEAFailedTelemedicina").Value,

                Data = new
                {
                    Beneficiaries = application.MedPersons.Select(m => new {
                        NameBeneficiary = m.Name,
                        DocumentTypeBeneficiary = m.DocumentType switch
                        {
                            76 => "Cédula de ciudadania",
                            77 => "Cédula de extranjeria",
                            _ => "Tarjeta Identidad"
                        },
                        DocumentNumberBeneficiary = m.DocumentNumber,
                        EmailBeneficiary = m.Email,
                        PhoneBeneficiary = m.PhoneNumber
                    }),
                    Name = application.Name,
                    DocumentNumber = application.DocumentNumber,
                    Email = application.Email,
                    Phone = application.Phone,
                    Amount = application.Amount / 100,
                    PaymentDate = CreatedTransactionDate.ToString("dd/MM/yyyy HH:mm"),
                    DocumentType = application.DocumentType switch
                    {
                        76 => "Cédula de ciudadania",
                        77 => "Cédula de extranjeria",
                        _ => "Tarjeta Identidad"
                    },
                    Frecuency = application.PaymentOcurrencyId switch
                    {
                        (int)MethodPaymentTimeEnum.Mensual => "Mensual",
                        (int)MethodPaymentTimeEnum.Semestral => "Semestral",
                        _ => "Anual"
                    }
                }
            };

            await SendGridEmailSender.SendEmailAsync(sendEmail);
        }
    }
}
