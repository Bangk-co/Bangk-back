using Bangk.Common.Dto;
using Bangk.Common.Validators;
using Bangk.Domain.Contract.Client;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;


namespace Bangk.Domain.Implementation.Client
{
    public class UpdateClientService : IUpdateClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IValidator<ClientUpdateInfoDto> _updateClientValidator;
        public UpdateClientService(IClientRepository clientRepository, IValidator<ClientUpdateInfoDto> updateClientValidator)
        {
            _clientRepository = clientRepository;
            _updateClientValidator = updateClientValidator;
        }
       
        public async Task<ResponseDto<ClientUpdateInfoDto>> Execute(int clientId, ClientUpdateInfoDto updateClient)
        {
            var result = new ResponseDto<ClientUpdateInfoDto>();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var validationResult = await _updateClientValidator.ValidateAsync(updateClient);

                if (!validationResult.IsValid)
                {
                    result.HasErrors = true;
                    foreach (var error in validationResult.Errors)
                        result.Errors.Add(error.ErrorMessage);
                }
                else
                {
                    var existClient = await _clientRepository.GetById(clientId);

                    if (existClient == null)
                    {
                        result.HasErrors = true;
                        result.Errors.Add("Cliente no encontrado");
                    }
                    else
                    {
                        existClient.Name = updateClient.Name;
                        existClient.LastName = updateClient.LastName;
                        existClient.DocumenIdentificationTypeId = updateClient.DocumentTypeId;
                        existClient.IdentificationNumber = updateClient.IdentificationNumber;
                        existClient.DocumentIssueDate = updateClient.DocumentIssueDate;
                        existClient.Email = updateClient.Email;
                        existClient.CountryCodePhoneId = updateClient.CountryCode;
                        existClient.PhoneNumber = updateClient.PhoneNumber;
                        existClient.BirthDate = updateClient.BirthDate;
                        existClient.GenderId = updateClient.GenderId;

                        await _clientRepository.Update(existClient);

                        result.Result = updateClient;
                    }
                }

                scope.Complete();

            }

            return result;

        }
    }
}
