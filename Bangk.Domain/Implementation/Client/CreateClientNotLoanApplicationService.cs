using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.Client;
using Bangk.Domain.Contract.Files;
using Bangk.Repositories.Repositories;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Client
{
    public class CreateClientNotLoanApplicationService : ICreateClientNotLoanApplicationService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IValidator<CreateClientNotLoanApplicationDto> _createClientValidator;

        public CreateClientNotLoanApplicationService(IClientRepository repository, IValidator<CreateClientNotLoanApplicationDto> createClientValidator)
        {
            _clientRepository = repository;
            _createClientValidator = createClientValidator;
        }

        public async Task<ResponseDto<CreateClientNotLoanApplicationDto>> Execute(CreateClientNotLoanApplicationDto createClientDto)
        {
            var result = new ResponseDto<CreateClientNotLoanApplicationDto>
            {
                Result = new CreateClientNotLoanApplicationDto()
            };

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
                LastName = createClientDto.LastName,
                Name = createClientDto.Name,
                PermissionTreatPersonalDataDate = createClientDto.PermissionTreatPersonalDataDate,
                ValidateClientOtp = createClientDto.ValidateClientOtp,
                State = (int)ClientStateEnum.Potencial,
            };

            await _clientRepository.Create(client);
            createClientDto.ClientId = client.Id;
            result.Result = createClientDto;
            return result;
        }
    }
}
