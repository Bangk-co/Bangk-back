using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.PetEnsuranceApplication;
using Bangk.Repositories.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Bangk.Domain.Implementation.PetEnsuranceApplication
{
    public class CreatePetEnsuranceApplicationService : ICreatePetEnsuranceApplicationService
    {
        private readonly IPetEnsuranceApplicationRepository _petEnsuranceApplicationRepository;
        private readonly IPetRepository _petRepository;
        public CreatePetEnsuranceApplicationService(IPetEnsuranceApplicationRepository petEnsuranceApplicationRepository, IPetRepository petRepository)
        {
            _petEnsuranceApplicationRepository = petEnsuranceApplicationRepository;
            _petRepository = petRepository;
        }

        public async Task<ResponseDto<int>> Execute(PetEnsuranceDto petEnsuranceDto)
        {
            var result = new ResponseDto<int>
            {
                Result = 0
            };

            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

            var petEnsuranceApplication = new Repositories.Entities.PetEnsuranceApplication()
            {
                Name = petEnsuranceDto.Name,
                DocumentNumber = petEnsuranceDto.DocumentNumber,
                DocumentTypeId = petEnsuranceDto.DocumentTypeId,
                Email = petEnsuranceDto.Email,
                MethodPayment = petEnsuranceDto.MethodPaymentId,
                PhoneNumber = petEnsuranceDto.PhoneNumber,
                Status = (int)InsuranceStatesEnum.PendingToCreate,
                TreatPersonalDataDate = petEnsuranceDto.TreatPersonalDataDate,
                
            };

            await _petEnsuranceApplicationRepository.Create(petEnsuranceApplication);
            var pets = new List<Repositories.Entities.Pet>();
            if(petEnsuranceDto.Pets.Count > 0)
            {
                foreach (var item in petEnsuranceDto.Pets)
                {
                    var petAdd = new Repositories.Entities.Pet
                    {
                        Age = item.Age,
                        Breed = item.Breed,
                        Color = item.Color,
                        Name = item.Name,
                        PetTypeId = item.PetTypeId,
                        PetEnsuranceApplicationId = petEnsuranceApplication.Id
                    };

                    pets.Add(petAdd);
                }
            }

            if (pets.Count > 0)
            { 
                await _petRepository.CreateBulk(pets);
            }

            result.Result = petEnsuranceApplication.Id;
            scope.Complete();
            return result;
        }
    }
}
