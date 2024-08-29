using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.Client;
using Bangk.Domain.Contract.Files;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Drawing;
using System.Drawing.Text;
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
        private readonly ISettingRepository _settingRepository;

        public CreateClientService(IClientRepository repository, IValidator<CreateClientDto> createClientValidator, IUploadFileService uploadFileService, 
            IConfiguration configuration, ILoanApplicationRepository loanApplicationRepository, ILoanApplicationDocumentRepository loanApplicationDocumentRepository, 
            IUserRepository userRepository, ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository, ISettingRepository settingRepository)
        {
            _clientRepository = repository;
            _createClientValidator = createClientValidator;
            _uploadFileService = uploadFileService;
            _configuration = configuration;
            _loanApplicationRepository = loanApplicationRepository;
            _loanApplicationDocumentRepository = loanApplicationDocumentRepository;
            _userRepository = userRepository;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _settingRepository = settingRepository;
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
                result.Errors.Add($"El cliente ingresado ya existe");
                return result;
            }

            var client = new Repositories.Entities.Client()
            {
                Email = createClientDto.Email.Trim(),
                IdentificationNumber = createClientDto.IdentificationNumber,
                DocumenIdentificationTypeId = createClientDto.DocumenIdentificationTypeId,
                BirthDate = createClientDto.BirthDate,
                CountryCodePhoneId = createClientDto.CountryCodePhoneId,
                GenderId = createClientDto.GenderId,
                LastName = createClientDto.LastName,
                Name = createClientDto.Name,
                PhoneNumber = createClientDto.PhoneNumber,
                PermissionTreatPersonalDataDate = createClientDto.PermissionTreatPersonalDataDate,
                ValidateClientOtp = createClientDto.ValidateClientOtp,
                State = (int)ClientStateEnum.Potencial,
            };

            await _clientRepository.Create(client);
            var userReview = await _userRepository.GetUserWithLessApplication();
            var loanApplication = new Repositories.Entities.LoanApplication
            {
                ClientId = client.Id,
                LoanBalance = createClientDto.LoadAplicacionDto.LoanBalance,
                LoanNumber = createClientDto.LoadAplicacionDto.LoanNumber,
                LoanTypeId = createClientDto.LoadAplicacionDto.LoanTypeId,
                BankId = createClientDto.LoadAplicacionDto.BankId,
                ReviewerId = userReview.Id
            };

            

            await _loanApplicationRepository.Create(loanApplication);
            var file = await _uploadFileService.Execute(createClientDto.IdentificationFile, _configuration.GetSection("AzureStorage:ContainerName").Value);
            var identificationFileUrl = file.Result;
            var identificationDocument = new Repositories.Entities.LoanApplicationDocument
            {
                LoanApplicationId = loanApplication.Id,
                Name = createClientDto.IdentificationFile.FileName,  
                DocumentType = (int)DocumentType.Identification,  
                Url = identificationFileUrl  
            };

            await _loanApplicationDocumentRepository.Create(identificationDocument);
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

            var smlv = int.Parse((await _settingRepository.GetById((int)SMLVEnum.SMLV)).Code);
            var smlvNumber = createClientDto.LoadAplicacionDto.LoanBalance / smlv;
            if(smlvNumber > 135)
            {
                loadApplicationStateHistory.Observations = "Valor del préstamo supera los 135 SMMLV";
            }

            await _loanApplicationStateHistoryRepository.Create(loadApplicationStateHistory);
            result.Result = client.Id;

            await SendNotification(userReview.Email, loanApplication.Id, client.Name);
            scope.Complete();
            return result;
        }

        private async Task SendNotification(string email, int applicationId, string clientName)
        {
            var url = _configuration.GetSection("LoanApplicationAssigned:UrlFront").Value;
            var sendEmail = new SendGridEmailDto
            {
                email = email,
                From = _configuration.GetSection("SendGridEmailSender:From").Value,
                Key = _configuration.GetSection("SendGridEmailSender:Key").Value,
                Name = _configuration.GetSection("SendGridEmailSender:Name").Value,
                TemplateId = _configuration.GetSection("SendGridEmailSender:LoanApplicationAssigned").Value,
                Data = new
                {
                    Cliename = clientName,
                    UrlCallback = url + $"?loanApplicationId={applicationId}"
                }
            };

            await SendGridEmailSender.SendEmailAsync(sendEmail);
        }
    }
}
