using Bangk.Common.Dto;
using Bangk.Domain.Contract.Client;
using Bangk.Repositories.Repositories;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Client
{
    public class ValidateClientService : IValidateClientService
    {
        private readonly IClientRepository _clientRepository;

        public ValidateClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<ResponseDto<bool>> Execute(string documentNumber, string email, int documentType)
        {
            var result = new ResponseDto<bool>
            {
                Result = true,
                HasErrors = false,
            };

            var clientByEmail = await _clientRepository.GetClientByEmail(email);
            if (clientByEmail != null && (clientByEmail.DocumenIdentificationTypeId != documentType ||
                clientByEmail.IdentificationNumber != documentNumber))
            {
                result.Result = false;
                result.HasErrors = true;
                result.Errors.Add($"Lo sentimos, el correo electrónico ya está registrado y no coincide con el cliente que ingresas. Cambia el correo electrónico o verifica tu tipo y número de documento para continuar.");
                return result;
            }

            var clientByDocumentNumber = await _clientRepository.GetClientByIdentification(documentNumber);
            if(clientByDocumentNumber != null && clientByEmail != null)
            {
                if (clientByDocumentNumber.Id != clientByEmail.Id)
                {
                    result.Result = false;
                    result.HasErrors = true;
                    result.Errors.Add($"Lo sentimos, el correo electrónico ya está registrado y no coincide con el cliente que ingresas. Cambia el correo electrónico o verifica tu tipo y número de documento para continuar.");
                    return result;
                }
            }

            return result;
        }
    }
}
