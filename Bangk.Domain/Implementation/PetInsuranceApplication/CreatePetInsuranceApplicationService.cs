using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.PetInsuranceApplication;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Bangk.Domain.Implementation.PetInsuranceApplication
{
    public class CreatePetInsuranceApplicationService : ICreatePetInsuranceApplicationService
    {
        private readonly IPetInsuranceApplicationRepository _petInsuranceApplicationRepository;
        private readonly IPetRepository _petRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IUserRepository _userRepository;

        public CreatePetInsuranceApplicationService(IPetInsuranceApplicationRepository petInsuranceApplicationRepository, 
            IPetRepository petRepository, 
            IClientRepository clientRepository, 
            IUserRepository userRepository)
        {
            _petInsuranceApplicationRepository = petInsuranceApplicationRepository;
            _petRepository = petRepository;
            _clientRepository = clientRepository;
            _userRepository = userRepository;
        }

        public async Task<ResponseDto<int>> Execute(PetInsuranceDto petInsuranceDto)
        {
            var result = new ResponseDto<int>
            {
                Result = 0
            };

            bool invalidaAge = petInsuranceDto.Pets.Any(p => p.Age < 6 || p.Age > 120);

            if (invalidaAge)
            {
                result.HasErrors = true;
                result.Errors.Add($"La edad de las mascotas debe ser mayor a 6 meses y menor a 10 años");
                return result;
            }

            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

            int clientId = 0;
            var clientExist = await _clientRepository.GetClientByIdentificationNumberDocumentTypeEmail(petInsuranceDto.DocumentTypeId, petInsuranceDto.DocumentNumber, petInsuranceDto.Email);

            if (clientExist == null)
            {
                var clientExistByDocument = await _clientRepository.GetClientByIdentificationNumberAndType(petInsuranceDto.DocumentTypeId, petInsuranceDto.DocumentNumber);
                if (clientExistByDocument == null)
                {
                    var client = new Repositories.Entities.Client
                    {
                        Email = petInsuranceDto.Email,
                        PermissionTreatPersonalDataDate = petInsuranceDto.TreatPersonalDataDate,
                        DocumenIdentificationTypeId = petInsuranceDto.DocumentTypeId,
                        IdentificationNumber = petInsuranceDto.DocumentNumber,
                        LastName = petInsuranceDto.LastName,
                        Name = petInsuranceDto.Name,
                        CountryCodePhoneId = petInsuranceDto.CountryCodePhoneId,
                        PhoneNumber = petInsuranceDto.PhoneNumber,
                    };

                    await _clientRepository.Create(client);
                    clientId = client.Id;
                }
                else
                {
                    clientExistByDocument.Email = petInsuranceDto.Email;
                    clientExistByDocument.PermissionTreatPersonalDataDate = petInsuranceDto.TreatPersonalDataDate;
                    clientExistByDocument.DocumenIdentificationTypeId = petInsuranceDto.DocumentTypeId;
                    clientExistByDocument.IdentificationNumber = petInsuranceDto.DocumentNumber;
                    clientExistByDocument.LastName = petInsuranceDto.LastName;
                    clientExistByDocument.Name = petInsuranceDto.Name;
                    clientExistByDocument.CountryCodePhoneId = petInsuranceDto.CountryCodePhoneId;
                    clientExistByDocument.PhoneNumber = petInsuranceDto.PhoneNumber;
                    await _clientRepository.Update(clientExistByDocument);
                    clientId = clientExistByDocument.Id;
                } 
            }
            else
            {
                clientId = clientExist.Id;
            }

            var userReview = await _userRepository.GetUserWithLessApplication();

            var petInsuranceApplication = new Repositories.Entities.PetInsuranceApplication()
            {
                ClientId = clientId,
                PaymentOcurrencyId = petInsuranceDto.PaymentOcurrencyId,
                Status = (int)InsuranceStatesEnum.PendingToApprove,
                ReviewerId = userReview.Id
            };

            await _petInsuranceApplicationRepository.Create(petInsuranceApplication);
            var nextPaymentDate = petInsuranceApplication.PaymentOcurrencyId switch
            {
                (int)MethodPaymentTimeEnum.Mensual => petInsuranceApplication.CreationDate.AddMonths(1),
                (int)MethodPaymentTimeEnum.Semestral => petInsuranceApplication.CreationDate.AddMonths(6),
                _ => petInsuranceApplication.CreationDate.AddYears(1)
            };

            petInsuranceApplication.NextPaymentDate = nextPaymentDate;
            await _petInsuranceApplicationRepository.Update(petInsuranceApplication);

            var pets = new List<Repositories.Entities.Pet>();
            if (petInsuranceDto.Pets.Count > 0)
            {
                int order = 1;
                foreach (var item in petInsuranceDto.Pets)
                {
                    var petAdd = new Repositories.Entities.Pet
                    {
                        Age = item.Age,
                        Breed = item.Breed,
                        Color = item.Color,
                        Name = item.Name,
                        PetTypeId = item.PetTypeId,
                        PetInsuranceApplicationId = petInsuranceApplication.Id,
                        Order = order
                    };

                    pets.Add(petAdd);
                    order++;
                }
            }

            if (pets.Count > 0)
            {
                await _petRepository.CreateBulk(pets);
            }

            result.Result = petInsuranceApplication.Id;
            scope.Complete();
            return result;
        }
    }
}
