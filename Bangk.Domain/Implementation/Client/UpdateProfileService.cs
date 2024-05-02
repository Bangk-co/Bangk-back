using Bangk.Common.Dto;
using Bangk.Domain.Contract.Client;
using Bangk.Repositories.Repositories;
using FluentValidation;
using System.Threading.Tasks;


namespace Bangk.Domain.Implementation.Client
{
    public class UpdateProfileService : IUpdateProfileService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IValidator<UpdateProfileDto> _updateClientValidator;
        public UpdateProfileService(IClientRepository clientRepository, IValidator<UpdateProfileDto> updateClientValidator)
        {
            _clientRepository = clientRepository;
            _updateClientValidator = updateClientValidator;
        }

        public async Task<ResponseDto<UpdateProfileDto>> Execute(UpdateProfileDto updateClient)
        {
            var result = new ResponseDto<UpdateProfileDto>();
            var validationResult = await _updateClientValidator.ValidateAsync(updateClient);
            if (!validationResult.IsValid)
            {
                result.HasErrors = true;
                foreach (var error in validationResult.Errors)
                    result.Errors.Add(error.ErrorMessage);
                return result;
            }

            var client = await _clientRepository.GetById(updateClient.ClientId);
            if (client == null)
            {
                result.HasErrors = true;
                result.Errors.Add("Cliente no encontrado");
                return result;
            }

            client.Name = updateClient.Name;
            client.LastName = updateClient.LastName;
            client.DocumenIdentificationTypeId = updateClient.DocumentTypeId;
            client.IdentificationNumber = updateClient.IdentificationNumber;
            client.DocumentIssueDate = updateClient.DocumentIssueDate;
            client.CountryCodePhoneId = updateClient.CountryCode;
            client.PhoneNumber = updateClient.PhoneNumber;
            client.BirthDate = updateClient.BirthDate;
            client.GenderId = updateClient.GenderId;
            await _clientRepository.Update(client);
            result.Result = updateClient;
            return result;
        }
    }
}
