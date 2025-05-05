using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<List<PaymentJobDto>> GetPetAssistencePendingToPay(DateTime dateTime)
        {
            var query = from p in Context.PetInsuranceApplication
                        join t in Context.ApplicationTransaction on p.TransactionReference equals t.Reference
                        join c in Context.Clients on p.ClientId equals c.Id
                        where p.NextPaymentDate.Value.Date == dateTime.Date && p.Status == (int)InsuranceStatesEnum.Approve && t.Status == "APPROVED"
                        select new PaymentJobDto
                        {
                            ApplicationId = p.Id,
                            ClientId = p.ClientId,
                            NextPaymentDate = p.NextPaymentDate,
                            TokenPaymentSource = p.TokenPaymentSource,
                            TransactionReference = p.TransactionReference,
                            Amount = t.Amount,
                            PaymentInstallments = t.PaymentInstallments,
                            Email = c.Email,
                        };

            return await query.ToListAsync();
        }

    }
}
