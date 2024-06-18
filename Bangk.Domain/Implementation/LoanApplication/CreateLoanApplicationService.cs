using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Common.Validators;
using Bangk.Domain.Contract.Files;
using Bangk.Domain.Contract.LoanApplication;
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

        public CreateLoanApplicationService(ILoanApplicationRepository LoanApplicationRepository, IUserRepository userRepository,
            IUploadFileService uploadFileService, IConfiguration configuration, ILoanApplicationDocumentRepository loanApplicationDocumentRepository,
            ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository)
        {
            _loanApplicationRepository = LoanApplicationRepository;
            _userRepository = userRepository;
            _uploadFileService = uploadFileService;
            _configuration = configuration;
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _loanApplicationDocumentRepository = loanApplicationDocumentRepository;
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

            var user = await _userRepository.GetUserWithLessApplication();
            var loanApplication = new Repositories.Entities.LoanApplication
            {
                ClientId = createLoadAplicacionDto.ClientId,
                LoanNumber = createLoadAplicacionDto.LoanNumber,
                LoanTypeId = createLoadAplicacionDto.LoanTypeId,
                BankId = createLoadAplicacionDto.BankId,
                ReviewerId = user.Id
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
            scope.Complete();
            return result;
        }
    }
}
