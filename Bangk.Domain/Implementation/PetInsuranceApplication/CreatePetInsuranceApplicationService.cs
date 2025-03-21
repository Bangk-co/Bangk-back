using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.PetInsuranceApplication;
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
            var clientExist = await _clientRepository.ExistClientByEmailAndIdentificationNumber(petInsuranceDto.Email, petInsuranceDto.DocumentNumber);

            if (!clientExist)
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
                var client = await _clientRepository.GetClientByEmailAndIdentificationNumber(petInsuranceDto.Email, petInsuranceDto.DocumentNumber);
                clientId = client.Id;
            }

            var userReview = await _userRepository.GetUserWithLessApplication();

            var petInsuranceApplication = new Repositories.Entities.PetInsuranceApplication()
            {
                ClientId = clientId,
                PaymentOcurrencyId = petInsuranceDto.PaymentOcurrencyId,
                Status = (int)InsuranceStatesEnum.PendingToCreate,
                ReviewerId = userReview.Id
            };

            await _petInsuranceApplicationRepository.Create(petInsuranceApplication);
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
