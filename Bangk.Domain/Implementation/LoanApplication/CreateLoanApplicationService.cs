using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Common.Validators;
using Bangk.Domain.Contract.Files;
using Bangk.Domain.Contract.LoanApplication;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Transactions;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class CreateLoanApplicationService : ICreateLoanApplicationService
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUploadFileService _uploadFileService;
        private readonly IConfiguration _configuration;
        private readonly ILoanApplicationDocumentRepository _loanApplicationDocumentRepository;
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        private readonly IClientRepository _clientRepository;
        public CreateLoanApplicationService(ILoanApplicationRepository LoanApplicationRepository, IUserRepository userRepository,
            IUploadFileService uploadFileService, IConfiguration configuration, ILoanApplicationDocumentRepository loanApplicationDocumentRepository,
            ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository, IClientRepository clientRepository)
        {
            _loanApplicationRepository = LoanApplicationRepository;
            _userRepository = userRepository;
            _uploadFileService = uploadFileService;
            _configuration = configuration;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _loanApplicationDocumentRepository = loanApplicationDocumentRepository;
            _clientRepository = clientRepository;
        }

        public async Task<ResponseDto<int>> Execute(NewLoadAplicacionDto createLoadAplicacionDto)
        {
            var result = new ResponseDto<int>
            {
                Result = 0
            };

            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);
            //var validationResult = await _createClientValidator.ValidateAsync(createClientDto);
            //if (!validationResult.IsValid)
            //{
            //    result.HasErrors = true;
            //    foreach (var error in validationResult.Errors)
            //        result.Errors.Add(error.ErrorMessage);
            //    return result;
            //}

            var userReview = await _userRepository.GetUserWithLessApplication();
            var loanApplication = new Repositories.Entities.LoanApplication
            {
                ClientId = createLoadAplicacionDto.ClientId,
                LoanNumber = createLoadAplicacionDto.LoanNumber,
                LoanTypeId = createLoadAplicacionDto.LoanTypeId,
                BankId = createLoadAplicacionDto.BankId,
                ReviewerId = userReview.Id
            };

            await _loanApplicationRepository.Create(loanApplication);

            var file = await _uploadFileService.Execute(createLoadAplicacionDto.BankStatementFile, _configuration.GetSection("AzureStorage:ContainerName").Value);
            var loanApplicationDocument = new Repositories.Entities.LoanApplicationDocument
            {
                LoanApplicationId = loanApplication.Id,
                Name = createLoadAplicacionDto.BankStatementFile.FileName,
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
            result.Result = loanApplication.Id;
            var client =  await _clientRepository.GetById(createLoadAplicacionDto.ClientId);
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
                    UrlCallback = url + $"&loanApplicationId={applicationId}"
                }
            };

            await SendGridEmailSender.SendEmailAsync(sendEmail);
        }
    }
}
