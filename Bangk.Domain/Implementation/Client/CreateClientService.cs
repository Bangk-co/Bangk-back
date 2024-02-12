using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.Client;
using Bangk.Domain.Contract.Files;
using Bangk.Repositories.Repositories;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Transactions;

namespace Bangk.Domain.Implementation.Client
{
    public class CreateClientService : ICreateClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IValidator<CreateClientDto> _createClientValidator;
        private readonly IUploadFileService _uploadFileService;
        private readonly IConfiguration _configuration;
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILoanApplicationDocumentRepository _loanApplicationDocumentRepository;
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        public CreateClientService(IClientRepository repository, IValidator<CreateClientDto> createClientValidator, IUploadFileService uploadFileService, 
            IConfiguration configuration, ILoanApplicationRepository loanApplicationRepository, ILoanApplicationDocumentRepository loanApplicationDocumentRepository, 
            IUserRepository userRepository, ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository)
        {
            _clientRepository = repository;
            _createClientValidator = createClientValidator;
            _uploadFileService = uploadFileService;
            _configuration = configuration;
            _loanApplicationRepository = loanApplicationRepository;
            _loanApplicationDocumentRepository = loanApplicationDocumentRepository;
            _userRepository = userRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
        }

        public async Task<ResponseDto<int>> Execute(CreateClientDto createClientDto)
        {
            var result = new ResponseDto<int>
            {
                Result = 0
            };

            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);
            var validationResult = await _createClientValidator.ValidateAsync(createClientDto);
            if (!validationResult.IsValid)
            {
                result.HasErrors = true;
                foreach (var error in validationResult.Errors)
                    result.Errors.Add(error.ErrorMessage);
                return result;
            }

            var clientExist = await _clientRepository.ExistClientByEmailAndIdentificationNumber(createClientDto.Email, createClientDto.IdentificationNumber);
            if (clientExist)
            {
                result.HasErrors = true;
                result.Errors.Add($"El client ingresado ya existe");
                return result;
            }

            var client = new Repositories.Entities.Client()
            {
                Email = createClientDto.Email,
                IdentificationNumber = createClientDto.IdentificationNumber,
                DocumenIdentificationTypeId = createClientDto.DocumenIdentificationTypeId,
                BirthDate = createClientDto.BirthDate,
                CountryCodePhoneId = createClientDto.CountryCodePhoneId,
                GenderId = createClientDto.GenderId,
                LastName = createClientDto.LastName,
                Name = createClientDto.Name,
                PhoneNumber = createClientDto.PhoneNumber,
                PermissionTreatPersonalDataDate = createClientDto.PermissionTreatPersonalDataDate,
                State = (int)ClientStateEnum.Potencial,
            };

            var file = await _uploadFileService.Execute(createClientDto.IdentificationFile, _configuration.GetSection("AzureStorage:ContainerName").Value);
            client.IdentificationFileUrl = file.Result;
            await _clientRepository.Create(client);
            var user = await _userRepository.GetUserWithLessApplication();

            var loanApplication = new Repositories.Entities.LoanApplication
            {
                ClientId = client.Id,
                LoanBalance = createClientDto.LoadAplicacionDto.LoanBalance,
                LoanNumber = createClientDto.LoadAplicacionDto.LoanNumber,
                LoanTypeId = createClientDto.LoadAplicacionDto.LoanTypeId,
                BankId = createClientDto.LoadAplicacionDto.BankId,
                ReviewerId = user.Id
            };

            await _loanApplicationRepository.Create(loanApplication);

            file = await _uploadFileService.Execute(createClientDto.LoadAplicacionDto.BankStatementFile, _configuration.GetSection("AzureStorage:ContainerName").Value);
            var loanApplicationDocument = new Repositories.Entities.LoanApplicationDocument
            {
                LoanApplicationId = loanApplication.Id,
                Name = createClientDto.LoadAplicacionDto.BankStatementFile.FileName,
                DocumentType = (int)DocumentType.BankStatement,
                Url = file.Result,
            };

            await _loanApplicationDocumentRepository.Create(loanApplicationDocument);
            var loadApplicationStateHistory = new Repositories.Entities.LoanApplicationStateHistory
            {
                LoanApplicationId = loanApplication.Id,
                Observations = string.Empty,
                StateId = (int)ApplicationStateHistoryEnum.SolicitudCreada,
            };

            await _loanApplicationStateHistoryRepository.Create(loadApplicationStateHistory);
            result.Result = client.Id;
            scope.Complete();
            return result;
        }
    }
}
