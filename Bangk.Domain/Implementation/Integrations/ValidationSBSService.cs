using Bangk.Common.Enums;
using Bangk.Domain.Contract.Integrations;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using ValidacionOFAC;

namespace Bangk.Domain.Implementation.Integrations
{
    public class ValidationSBSService : IValidationSBSService
    {
        private readonly ILoanQuoteRepository _loanQuoteRepository;
        private readonly IConfiguration _configuration;
        private readonly IClientRepository _clientRepository;
        private readonly ILoanInsurancePolicyRepository _loanInsurancePolicyRepository;
        private readonly IValidationSBSLogRepository _validationSBSLogRepository;
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        private readonly IScheduleIntegrationRepository _scheduleIntegrationRepository;

        public ValidationSBSService(IConfiguration configuration, ILoanInsurancePolicyRepository loanInsurancePolicyRepository,
           ILoanQuoteRepository loanQuoteRepository, IClientRepository clientRepository, IValidationSBSLogRepository validationSBSLogRepository,
           ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository, IScheduleIntegrationRepository scheduleIntegrationRepository)
        {
            _loanQuoteRepository = loanQuoteRepository;
            _configuration = configuration;
            _clientRepository = clientRepository;
            _loanInsurancePolicyRepository = loanInsurancePolicyRepository;
            _validationSBSLogRepository = validationSBSLogRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _scheduleIntegrationRepository = scheduleIntegrationRepository;
        }

        public async Task Execute(int loanApplicationId)
        {
            var insuranceName = await _loanQuoteRepository.GetInsuraceNameByLoanApplicatioId(loanApplicationId);
            if (string.Equals(insuranceName, "SBS"))
            {
                ValidacionOFACClient clientOFAC = new ValidacionOFACClient(ValidacionOFACClient.EndpointConfiguration.BasicHttpsBinding_IValidacionOFAC);
                var client = await _clientRepository.GetClientByLoanApplicationId(loanApplicationId);
                var clientSBSInf = await _loanInsurancePolicyRepository.GetClientSBSInfoDtoyByLoanApplicationId(loanApplicationId);
                var responseValidation = clientOFAC.ValidarOFAC(_configuration.GetSection("IntegrationSBS:Token").Value, _configuration.GetSection("IntegrationSBS:ApplicationName").Value,
                    client.DocumenIdentificationTypeId == 76 ? TipoDocumento.Cedula : TipoDocumento.Cedula_Extranjeria, client.IdentificationNumber, client.Name, client.LastName,
                    clientSBSInf.BirthCountrySBSId, clientSBSInf.CountryNationalitySBSId);

                if (responseValidation.RegistroValidado)
                {
                    var OFACRestriction = responseValidation.TipoRestriccionOfac == 1 ? "Presenta novedad en lista restrictiva" : responseValidation.TipoRestriccionOfac == 0
                        ? "Cliente apto en lista restrictiva" : "Cliente no apto para emisión de póliza";
                    await SaveLoanHistory(loanApplicationId, OFACRestriction, (int)ApplicationStateHistoryEnum.EstudioListaRestrictivaSBS);

                    //llama otro proceso de integracion
                }
                else
                {
                    var OFACRestriction = responseValidation.DescripcionError;
                    //await SaveLoanHistory(loanApplicationId, OFACRestriction, (int)ApplicationStateHistoryEnum.EstudioListaRestrictivaSBS);
                    await CreateNextTrySBS(loanApplicationId);
                }

                await SaveValidacionOFACLog(responseValidation);
            }
        }

        private async Task SaveValidacionOFACLog(clsResultadoValidacion clsResultadoValidacion)
        {
            var validationOfac = new ValidationSBSLog
            {
                DetalleIdsErrores = clsResultadoValidacion.DetalleIdsErrores,
                DescripcionError = clsResultadoValidacion.DescripcionError,
                DetalleListaOfac = clsResultadoValidacion.DetalleListaOFAC,
                OrigenRestriccionOfac = clsResultadoValidacion.OrigenRestriccionOfac,
                OrigenRestriccionOfacDesc = clsResultadoValidacion.OrigenRestriccionOfacDesc,
                RegistroValidado = clsResultadoValidacion.RegistroValidado,
                TipoRestriccionOfac = clsResultadoValidacion.TipoRestriccionOfac,
                TipoRestriccionOfacDesc = clsResultadoValidacion.TipoRestriccionOfacDesc
            };

            await _validationSBSLogRepository.Create(validationOfac);
        }

        private async Task SaveLoanHistory(int loanApplicationId, string observation, int state)
        {
            var loanApplicationHistory = new Repositories.Entities.LoanApplicationStateHistory
            {
                CreationDate = DateTime.Now,
                LoanApplicationId = loanApplicationId,
                Observations = observation ?? string.Empty,
                StateId = state
            };

            await _loanApplicationStateHistoryRepository.Create(loanApplicationHistory);
        }

        private async Task CreateNextTrySBS(int loanApplicationId)
        {
            var nextExecution = new ScheduleIntegration
            {
                CountFails = 1,
                NextTryDate = DateTime.UtcNow.AddMinutes(1),
                IntegrationType = 1,
                LoanApplicationId = loanApplicationId
            };

            await _scheduleIntegrationRepository.Create(nextExecution);
        }
    }
}
