using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class PetInsuranceApplicationRepository : GenericRepository<PetInsuranceApplication, PetInsuranceDto>, IPetInsuranceApplicationRepository
    {
        public PetInsuranceApplicationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PetInsuranceApplication> GetPetApplicationByReference(string reference)
        {
            return await Context.PetInsuranceApplication.FirstOrDefaultAsync(p => p.TransactionReference == reference);
        }

        public async Task<CreatePetAssistenceGEADto> GetPetAssistenceGEA(string reference)
        {
            var query = from a in Context.PetInsuranceApplication
                        join c in Context.Clients on a.ClientId equals c.Id
                        where a.TransactionReference == reference
                        select new CreatePetAssistenceGEADto
                        {
                            ApplicationId = a.Id,
                            CreatedDate = a.CreationDate,
                            DocumentNumber = c.IdentificationNumber,
                            DocumentType = c.DocumenIdentificationTypeId,
                            Email = c.Email,
                            Name = $"{c.Name}{c.LastName}",
                            PaymentOcurrencyId = a.PaymentOcurrencyId,
                            Phone = c.PhoneNumber,
                            Pets = Context.Pets.Where(p => p.PetInsuranceApplicationId == a.Id).Select(p => new CreatePetGEADto
                            {
                                Age = p.Age,
                                Breed = p.Breed,
                                Color = p.Color,
                                Name = p.Name,
                                Order = p.Order,
                                PetType = p.PetTypeId,
                            }).ToList(),
                        };

            return await query.FirstOrDefaultAsync();
        }
    }
}
