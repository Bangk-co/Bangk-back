using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.BackgroundTask
{
    public class ReportClientsSarlaft : IReportExcel
    {
        private readonly IClientRepository _clientRepository;
        private readonly SftpClient _sftpClient;
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;


        public ReportClientsSarlaft(IClientRepository clientRepository, SftpClient sftpClient, ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository)
        {
            _clientRepository = clientRepository;
            _sftpClient = sftpClient;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
        }

        public async Task CreateSarlaftReport()
        {
            
            string dateFile = DateTime.UtcNow.ToString("yyyyMMdd") + " Ingesta BangK.xlsx";
            string localFilePath = Path.Combine( dateFile);
            var stateloanApplication = ApplicationStateHistoryEnum.SolicitudEmisionPoliza;
            var eligibleClients = _clientRepository.GetReportClientsSarlaft(stateloanApplication);
            await GenerateExcel(localFilePath, eligibleClients);
            string remoteDirectory = "/Reporte_Sarlaft/"; 
            await UploadFileToSftp(localFilePath, remoteDirectory);

            var loanApplicationIds = _clientRepository.GetLoanApplicationIdsWithState(stateloanApplication);
            var clientListId = loanApplicationIds.ToList();
            foreach (var loanApplicationId in loanApplicationIds)
            {
                var newLoanApplicationStateHistory = new Repositories.Entities.LoanApplicationStateHistory
                {
                    CreationDate = DateTime.UtcNow,
                    LoanApplicationId = loanApplicationId,
                    Observations = string.Empty,
                    StateId = (int)ApplicationStateHistoryEnum.PolizaEmitida

                };

                await _loanApplicationStateHistoryRepository.Create(newLoanApplicationStateHistory);
            }
        }

        public async Task GenerateExcel(string dateFile, IQueryable<ClientReportDto> clients)
        {
            try
            {
                string path = dateFile;

                using (var package = new ExcelPackage(new FileInfo(path)))
                {

                    var worksheet = package.Workbook.Worksheets.Add("Reporte Sarlaft");

                    Console.WriteLine("Generando Titulos");


                    string[] titles = {  "COD.ASEG.","NOMBRE ASEGURADORA","RAMO","NOMBRE RAMO","CLASE","NOMBRE CLASE","CARATULA","NOMBRE CARATULA",
                        "PLAN","NOMBRE PLAN","CODIGO UNICO DE POLIZA","TIPO DOC.CLI.BANCO","NUM. DOC.CLI.BANCO","NOMBRE CLI. BANCO","APELLIDO CLI. BANCO",
                        "DIRECCION OFICINA.","DIRECCION RESIDENCIA","BARRIO","CIUDAD","DEPARTAMENTO","PAIS","TEL. OFICINA","TEL. RESIDENCIA","NUM. CELULAR",
                        "EMAIL","FECHA DE NACIMIENTO","SEXO","ESTADO CIVIL","No. DE HIJOS","TIPO DOC.ASEGURADO","NUM. DOC.ASEGURADO","NOMBRE ASEGURADO",
                        "APELLIDO ASEGURADO","ACTIVIDAD ECONOMICA","COD. OFIC. DE VENTA","NOMBRE DE OFIC. DE VENTA","CODIGO ASESOR","NOMBRE ASESOR","ZONA",
                        "COD. GERENTE ASIGNADO","NOMBRE GERENTE ASIGNADO","COD. CANAL DE VINCULACION","NOMBRE CANAL DE VINCULACION","TIPO DOC. CANAL VINCULACION",
                        "NUMERO DOC. CANAL VINCULACION","TIPO DE NOVEDAD","ESTADO POLIZA","CUOTAS EN MORA","DIAS EN MORA","TOMADOR","FECHA SOLICITUD CLIENTE",
                        "INICIO VIGENCIA POLIZA","FIN VIGENCIA POLIZA","FECHA DE CANCELACION","RENOVACION AUTOMATICA","PERIODICIDAD DE PAGO",
                        "NOMBRE BENEFICIARIO 1","PARENTESCO 1","% PARTICIPACION 1","NOMBRE BENEFICIARIO 2","PARENTESCO 2","% PARTICIPACION 2",
                        "NOMBRE BENEFICIARIO 3","PARENTESCO 3","% PARTICIPACION 3","NOMBRE BENEFICIARIO 4","PARENTESCO 4","% PARTICIPACION 4",
                        "NOMBRE BENEFICIARIO 5","PARENTESCO 5","% PARTICIPACION 5","NOMBRE BENEFICIARIO 6","PARENTESCO 6","% PARTICIPACION 6",
                        "NOMBRE BENEFICIARIO 7","PARENTESCO 7","% PARTICIPACION 7","NOMBRE BENEFICIARIO 8","PARENTESCO 8","% PARTICIPACION 8",
                        "TIPO CUENTA RECAUDO","No CUENTA RECAUDO","No CUOTAS EN TC","VALOR ASEGURADO","VALOR PRIMA","NÚMERO DE PISOS","DIREC. INMUEBLE ASEGURADO",
                        "AÑOS DE CONSTRUCCIÓN","CIUDAD BIEN ASEGURADO","PLAZO PARA DESEMPLEO","PLACA","OBSERVACIONES","FECHA PROCESO DEL REPORTE","NUMERO CREDITO",
                        "ESTRUCTURA","CONTENIDOS","TODO RIESGO","ESCANER","ID_SPEEDY","% DISTRIBUCION ASEGU.","MASCOTA ESPECIE","MASCOTA SEXO","MASCOTA RAZA",
                        "MASCOTA MICROSHIP","MASCOTA No MICROSHIP","MASCOTA NOMBRE","MASCOTA FECHA DE NACIMIENTO","MASCOTA COLOR","MASCOTA TAMAÑO",
                        "FECHA DE PAGO","MOTIVO CIERRE","ID LLAMADA","POLIZA SYNERGY","COD. PLAN ANT","PLAN ANTERIOR","PRIMA ANTERIOR","PRIMA NO CAUSADA",
                        "PRIMA REINTEGRAR","TIPO DE CONSTRUCCIÓN"};

                    for (int i = 0; i < titles.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = titles[i];
                    }

                    int row = 2;
                    var clientList = clients.ToList();
                    int lastConsecutive = GetLastConsecutive();
                    

                    foreach (var client in clients)
                    {
                        worksheet.Cells[row, 1].Value = "3";
                        worksheet.Cells[row, 2].Value = "SBS SEGUROS COLOMBIA SA";
                        worksheet.Cells[row, 3].Value = "128";
                        worksheet.Cells[row, 4].Value = "VIDA GRUPO";
                        worksheet.Cells[row, 7].Value = "BK00002";
                        worksheet.Cells[row, 8].Value = "VIDA GRUPO BANGK";

                        DateTime birthDate = client.BirthDate ?? DateTime.Today;
                        int age = DateTime.Today.Year - birthDate.Year;

                        if (birthDate.Date > DateTime.Today.AddYears(-age))
                        {
                            age--;
                        }
                        string plan;
                        if (age >= 18 && age <= 50)
                        {
                            plan = "1";
                        }
                        else if (age >= 51 && age <= 64)
                        {
                            plan = "2";
                        }
                        else
                        {

                            plan = "Plan sin definir";
                        }

                        worksheet.Cells[row, 9].Value = plan;

                        
                        if (birthDate.Date > DateTime.Today.AddYears(-age))
                        {
                            age--;
                        }

                        string planName;
                        if (age >= 18 && age <= 50)
                        {
                            planName = "Vida I";
                        }
                        else if (age >= 51 && age <= 64)
                        {
                            planName = "Vida II";
                        }
                        else
                        {

                            planName = "Plan sin definir";
                        }
                        worksheet.Cells[row, 10].Value = planName;

                        worksheet.Cells[row, 11].Value = $"BK128{lastConsecutive}";
                        worksheet.Cells[row, 12].Value = client.DocumenIdentificationTypeId == (int)DocumenIdentificationTypeEnum.Cc ? 1 : client.DocumenIdentificationTypeId == (int)DocumenIdentificationTypeEnum.Ce ? 2 : null;
                        worksheet.Cells[row, 13].Value = client.IdentificationNumber;
                        worksheet.Cells[row, 14].Value = client.Name;
                        worksheet.Cells[row, 15].Value = client.LastName;
                        worksheet.Cells[row, 17].Value = client.Address;
                        worksheet.Cells[row, 18].Value = client.CityName;
                        worksheet.Cells[row, 19].Value = client.CityName;
                        worksheet.Cells[row, 20].Value = client.StateName;
                        worksheet.Cells[row, 21].Value = client.CountryName;
                        worksheet.Cells[row, 24].Value = client.PhoneNumber;
                        worksheet.Cells[row, 25].Value = client.Email;
                        worksheet.Cells[row, 26].Value = client.BirthDate?.ToString("yyyyMMdd");
                        worksheet.Cells[row, 30].Value = client.DocumenIdentificationTypeId == (int)DocumenIdentificationTypeEnum.Cc ? 1 : client.DocumenIdentificationTypeId == (int)DocumenIdentificationTypeEnum.Ce ? 2 : null;
                        worksheet.Cells[row, 31].Value = client.IdentificationNumber;
                        worksheet.Cells[row, 32].Value = client.Name;
                        worksheet.Cells[row, 33].Value = client.LastName;
                        worksheet.Cells[row, 46].Value = "AP";
                        worksheet.Cells[row, 50].Value = "BUILDING LIFE TECHNOLOGIES S.A.S. - 901647130-5";
                        worksheet.Cells[row, 51].Value = client.CreationDatePolicy.ToString("yyyyMMdd");
                        worksheet.Cells[row, 52].Value = DateTime.UtcNow.ToString("yyyyMMdd");
                        worksheet.Cells[row, 53].Value = DateTime.UtcNow.AddYears(1).ToString("yyyyMMdd");
                        worksheet.Cells[row, 56].Value = "1A";
                        worksheet.Cells[row, 57].Value = client.NameBank + " - " + client.NitBank;
                        worksheet.Cells[row, 58].Value = "Oneroso";
                        worksheet.Cells[row, 59].Value = "100%";

                        int beneficiaryColumn = 60;

                        foreach (var beneficiary in client.Beneficiary)
                        {
                            worksheet.Cells[row, beneficiaryColumn].Value = beneficiary.Name + " " + client.LastName;
                            worksheet.Cells[row, beneficiaryColumn + 1].Value = beneficiary.RelationshipId;
                            worksheet.Cells[row, beneficiaryColumn + 2].Value = beneficiary.PercentInherit;
                            beneficiaryColumn += 3;
                        }

                        worksheet.Cells[row, 84].Value = client.LoanBalance;
                        worksheet.Cells[row, 85].Value = client.YearlyFee;

                        row++;
                        lastConsecutive++;
                        SaveLastConsecutive(lastConsecutive);
                    }
                    package.Save();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en el método Crear Ingesta Bangk Polizas  -> {ex}");
            }
        }

        private int GetLastConsecutive()
        {
            string filePathC = "consecutivo.txt";

            if (File.Exists(filePathC))
            {
                string lastConsecutiveText = File.ReadAllText(filePathC);
                if (int.TryParse(lastConsecutiveText, out int lastConsecutive))
                {
                    return lastConsecutive;
                }
            }
            else
            {
                SaveLastConsecutive(10002);
                return 10002;

            }
            
            return 10002; 
        }

        
        private void SaveLastConsecutive(int lastConsecutive)
        {
            string filePathC = "consecutivo.txt";
            File.WriteAllText(filePathC, lastConsecutive.ToString());
        }


        public async Task CreateSarlaftReportLogs()
        {
            string dateFile = DateTime.UtcNow.ToString("yyyyMMdd") + " Ingesta con Logs de aceptacion BangK.xlsx";
            string localFilePath = Path.Combine(dateFile);
            var stateloanApplication = ApplicationStateHistoryEnum.SolicitudEmisionPoliza;
            var eligibleClients = _clientRepository.GetReportClientsSarlaftLogs(stateloanApplication);
            await GenerateExcelLogs(localFilePath, eligibleClients);
            string remoteDirectory = "/Reporte_Sarlaft/";
            await UploadFileToSftp(localFilePath, remoteDirectory);

        }


        public async Task GenerateExcelLogs (string dateFile, IQueryable<ClientReport2Dto> clients)
        {
            try
            {
                string path = dateFile;

                using (var package = new ExcelPackage(new FileInfo(path)))
                {

                    var worksheet = package.Workbook.Worksheets.Add("Ingesta con logs de aceptacion");

                    Console.WriteLine("Generando Titulos");


                    string[] titles = { "Fecha de Aceptacion de PTD","Fecha de Aceptacion de TyC", "Fecha de Ingreso con OTP", "Fecha de Ingreso con OTP", "Fecha de Ingreso con OTP (Si aplica)",
                    "Fecha de aceptacion de origen de fondos ( si aplica)", "Nombres","Apellidos","Tipo de documento","Numero de documento","Codigo del pais","Numero de telefono","Correo Electronico",
                    "País de nacimiento","Departamento/Estado de nacimiento","Ciudad de nacimiento","Nacionalidad","Estado civil","Número de hijos","Profesión","Actividad económica","Persona PPE",
                    "Familiares PPE","País de residencia","Departamento de residencia","Ciudad de residencia","Ingresos mensuales","Gastos mensuales","Valor de tus activos","Valor de tus pasivos",
                    "Origen de mis fondos","Cuál es tu peso en kilos? (kg)","¿Cuál es tu estatura en centímetros? (cm)","Te han diagnosticado alguna(s) de estas enfermedades, circunstancias o eventos?","Ampliar informacion de diagnosticos",
                    "¿Te han detectado la presencia de anticuerpos contra el virus V.I.H. productor del SIDA?","¿Te encuentras actualmente en exámenes, estudios, investigación o te han indicado la necesidad de alguna cirugía o tratamiento que no te hayas realizado aún?",
                    "Ampliar informacion de examanes","Tienes algún tipo de discapacidad, incapacidad o limitación física o mental?","Ampliar informacion de discapacidad",
                    "Has sido indiciado, investigado, procesado, inculpado, imputado, detenido y/o condenado por algún delito?","Tienes algún seguro de vida individual o de accidentes personales con Allianz o con cualquier aseguradora diferentes a aquellos que respaldan deudas en entidades financieras?",
                    "NOMBRE BENEFICIARIO 1","TIPO DE DOCUMENTO BENEFICIARIO 1","NUMERO DOCUMENTO BENEFICIARIO 1","PARENTESCO 1","% PARTICIPACION 1","NOMBRE BENEFICIARIO 2",
                    "TIPO DE DOCUMENTO BENEFICIARIO 2","NUMERO DOCUMENTO BENEFICIARIO 2","PARENTESCO 2","% PARTICIPACION 2","NOMBRE BENEFICIARIO 3","TIPO DE DOCUMENTO BENEFICIARIO 3",
                    "NUMERO DOCUMENTO BENEFICIARIO 3", "PARENTESCO 3","% PARTICIPACION 3","NOMBRE BENEFICIARIO 4","TIPO DE DOCUMENTO BENEFICIARIO 4","NUMERO DOCUMENTO BENEFICIARIO 4","PARENTESCO 4",
                    "% PARTICIPACION 4","NOMBRE BENEFICIARIO 5","TIPO DE DOCUMENTO BENEFICIARIO 5","NUMERO DOCUMENTO BENEFICIARIO 5","PARENTESCO 5","% PARTICIPACION 5","NOMBRE BENEFICIARIO 6",
                    "TIPO DE DOCUMENTO BENEFICIARIO 6", "NUMERO DOCUMENTO BENEFICIARIO 6","PARENTESCO 6","% PARTICIPACION 6","NOMBRE BENEFICIARIO 7","TIPO DE DOCUMENTO BENEFICIARIO 7",
                    "NUMERO DOCUMENTO BENEFICIARIO 7","PARENTESCO 7", "% PARTICIPACION 7","NOMBRE BENEFICIARIO 8","TIPO DE DOCUMENTO BENEFICIARIO 8","NUMERO DOCUMENTO BENEFICIARIO 8","PARENTESCO 8","% PARTICIPACION 8" };


                    for (int i = 0; i < titles.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = titles[i];
                    }

                    int row = 2;
                    var clientList = clients.ToList();

                    foreach (var client in clientList)
                    {
                        worksheet.Cells[row, 1].Value = client.PermissionTreatPersonalDataDate.ToString("yyyyMMdd");
                        worksheet.Cells[row, 2].Value = client.TermsConditionsAcceptanceQuote?.ToString("yyyyMMdd");
                        worksheet.Cells[row, 3].Value = client.ValidateClientOtp?.ToString("yyyyMMdd");
                        worksheet.Cells[row, 4].Value = client.AcceptQuoteOtp?.ToString("yyyyMMdd");
                        //Tercer otp pendiente
                        worksheet.Cells[row, 6].Value = client.AcceptanceOriginOfFunds?.ToString("yyyyMMdd");
                        worksheet.Cells[row, 7].Value = client.Name;
                        worksheet.Cells[row, 8].Value = client.LastName;
                        worksheet.Cells[row, 9].Value = client.DocumenIdentificationTypeId;
                        worksheet.Cells[row, 10].Value = client.IdentificationNumber;
                        worksheet.Cells[row, 11].Value = client.CountryCodePhoneId;
                        worksheet.Cells[row, 12].Value = client.PhoneNumber;
                        worksheet.Cells[row, 13].Value = client.Email;
                        worksheet.Cells[row, 14].Value = client.CountryName;
                        worksheet.Cells[row, 15].Value = client.StateName;
                        worksheet.Cells[row, 16].Value = client.CityName;
                        worksheet.Cells[row, 17].Value = client.CountryNationality;
                        worksheet.Cells[row, 18].Value = client.CivilStatus;
                        worksheet.Cells[row, 19].Value = client.NumberChildren;
                        worksheet.Cells[row, 20].Value = client.Occupation;
                        worksheet.Cells[row, 21].Value = client.OccupationActivity;
                        worksheet.Cells[row, 22].Value = client.IsPublicPerson.HasValue ? (client.IsPublicPerson.Value ? "Si" : "No") : ""; 
                        worksheet.Cells[row, 23].Value = client.HasBondPublicPerson.HasValue ? (client.IsPublicPerson.Value ? "Si" : "No") : "";
                        worksheet.Cells[row, 24].Value = client.ResidenceCountry;
                        worksheet.Cells[row, 25].Value = client.ResidenceState;
                        worksheet.Cells[row, 26].Value = client.ResidenceCity;
                        worksheet.Cells[row, 27].Value = client.MonthlyIncome;
                        worksheet.Cells[row, 28].Value = client.MonthlyExpenses;
                        worksheet.Cells[row, 29].Value = client.AssetValue;
                        worksheet.Cells[row, 30].Value = client.LiabilitiesValue;
                        worksheet.Cells[row, 31].Value = client.IncomeOrigin;
                        worksheet.Cells[row, 32].Value = client.Weight;
                        worksheet.Cells[row, 33].Value = client.Height;
                        worksheet.Cells[row, 34].Value =  _clientRepository.GetDiseasesNames(client.Diseases);
                        worksheet.Cells[row, 35].Value = client.DisabilityMoreInfo;
                        worksheet.Cells[row, 36].Value = client.HIVActive.HasValue ? (client.IsPublicPerson.Value ? "Si" : "No") : "";
                        worksheet.Cells[row, 37].Value = client.PlannedMedicalProcidure.HasValue ? (client.IsPublicPerson.Value ? "Si" : "No") : "";
                        worksheet.Cells[row, 38].Value = client.PlannedMedicalProcidureMoreInfo;
                        worksheet.Cells[row, 39].Value = client.Disability.HasValue ? (client.IsPublicPerson.Value ? "Si" : "No") : "";
                        worksheet.Cells[row, 40].Value = client.DisabilityMoreInfo;
                        worksheet.Cells[row, 41].Value = client.Convicted.HasValue ? (client.IsPublicPerson.Value ? "Si" : "No") : "";
                        worksheet.Cells[row, 42].Value = client.LifeEnsure.HasValue ? (client.IsPublicPerson.Value ? "Si" : "No") : "";
                        worksheet.Cells[row, 43].Value = client.NameBank;
                        worksheet.Cells[row, 44].Value = "NIT";
                        worksheet.Cells[row, 45].Value = client.NitBank;
                        worksheet.Cells[row, 46].Value = "Oneroso";
                        worksheet.Cells[row, 47].Value = "100%";


                        int beneficiaryColumn = 48;

                        foreach (var beneficiary in client.Beneficiary)
                        {
                            worksheet.Cells[row, beneficiaryColumn].Value = beneficiary.Name + " " + client.LastName;
                            worksheet.Cells[row, beneficiaryColumn + 1].Value = beneficiary.DocumenIdentificationTypeB;
                            worksheet.Cells[row, beneficiaryColumn + 2].Value = beneficiary.IdentificationNumberB;
                            worksheet.Cells[row, beneficiaryColumn + 3].Value = beneficiary.Relationship;
                            worksheet.Cells[row, beneficiaryColumn + 4].Value = beneficiary.PercentInherit;
                            beneficiaryColumn += 5;
                        }
                        row++;
                        
                    }
                    package.Save();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en el método Crear Ingesta con logs de aceptacion  -> {ex}");
            }
        }
        public async Task UploadFileToSftp(string localFilePath, string remoteDirectory)
        {
            try
            {
                using (var fileStream = new FileStream(localFilePath, FileMode.Open))
                {

                   _sftpClient.UploadFile(fileStream, Path.Combine(remoteDirectory, Path.GetFileName(localFilePath)));
                }
                File.Delete(localFilePath);

                Console.WriteLine("Archivo subido exitosamente al servidor SFTP.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir archivo al servidor SFTP: {ex.Message}");
            }
        }

    }    

}

