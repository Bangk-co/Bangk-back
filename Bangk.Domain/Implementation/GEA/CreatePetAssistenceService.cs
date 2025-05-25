using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Dto.MecharntsInfoDto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.GEA;
using Bangk.Repositories.Entities;
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

        public async Task<ResponseDto<bool>> Execute(string applicationReference, DateTime CreatedTransactionDate)
        {
            var token = await _getApplicationTokenGEAService.Execute();
            var baseUrl = _configuration.GetSection("GeaKeys:GeaUrl").Value;
            var cuentaId = int.Parse(_configuration["GeaKeys:IdCuenta"]);
            var planId = int.Parse(_configuration["GeaKeys:IdPlan"]);
            var application = await _petInsuranceApplicationRepository.GetPetAssistenceGEA(applicationReference);
            var count = 1;
            foreach (var item in application.Pets)
            {
                using (HttpClient client = new HttpClient())
                {
                    var clients = new ClientGEAPetDto();
                    var clientPets = new List<DatosWrapperDto>();
                    DatosWrapperDto datos = new DatosWrapperDto();
                    var tipoCveAfiliado = application.DocumentType switch
                    {
                        (int)DocumenIdentificationTypeEnum.Cc => 1,
                        (int)DocumenIdentificationTypeEnum.Ce => 2,
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
                        tipo_cveafiliado = 11,
                        cuenta = cuentaId,
                        plan = planId,
                        fecha_inicio_vigencia = fechaInicio,
                        fecha_fin_vigencia = fechaFin.ToString("yyyy-MM-dd"),
                        estado = "ACTIVO",
                        identificacion = application.DocumentNumber,
                        tipo_identificacion = tipoCveAfiliado,
                        nombres = application.Name,
                        email = application.Email.Trim(),
                        telefono_personal = application.Phone,
                    };

                    var datosExtra = new DataGEAExtrasDto
                    {
                        tipo_mascota = petType,
                        edad_mascota = item.Age,
                        raza_mascota = item.Breed,
                        color_mascota = item.Color,
                        nombre_mascota = item.Name
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

                    count++;
                }
            }


            return new ResponseDto<bool>();
        }

        private async Task SendEmailFailedCreationGEA(CreatePetAssistenceGEADto application, DateTime CreatedTransactionDate)
        {

            var url = _configuration.GetSection("LoanApplicationAssigned:UrlFront").Value;
            var sendEmail = new SendGridEmailDto
            {
                Email = _configuration.GetSection("GeaKeys:EmailErrorGea").Value,
                Cc = _configuration.GetSection("GeaKeys:EmailCopyErrorGea").Value,
                From = _configuration.GetSection("SendGridEmailSender:From").Value,
                Key = _configuration.GetSection("SendGridEmailSender:Key").Value,
                Name = _configuration.GetSection("SendGridEmailSender:Name").Value,
                TemplateId = _configuration.GetSection("SendGridEmailSender:GEAFailedMascotas").Value,

                Data = new
                {
                    Pets = application.Pets.Select(m => new {
                        Name = m.Name,
                        Age = m.Age,
                        Breed = m.Breed,
                        Color = m.Color,
                        PetType = m.PetType switch
                        {
                            (int)PetType.Cat => "Gato",
                            _ => "Perro"
                        }
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
