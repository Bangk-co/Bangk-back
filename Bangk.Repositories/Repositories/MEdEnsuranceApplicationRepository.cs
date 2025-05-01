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
    public class MEdEnsuranceApplicationRepository : GenericRepository<MedInsuranceApplication, MedEnsuranceApplicationDto>, IMedEnsuranceApplicationRepository
    {
        public MEdEnsuranceApplicationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<MedInsuranceApplication> GetMedEnsuranceApplicationByReference(string reference)
        {
            return await Context.MedInsuranceApplication.FirstOrDefaultAsync(p => p.TransactionReference == reference);
        }

        public async Task<CreateMedAssistenceGEADto> GetMedAssistenceGEA(string reference)
        {
            var query = from m in Context.MedInsuranceApplication
                        join c in Context.Clients on m.ClientId equals c.Id
                        where m.TransactionReference == reference
                        select new CreateMedAssistenceGEADto
                        {
                            CreatedDate = m.CreationDate,
                            ApplicationId = m.Id,
                            DocumentNumber = c.IdentificationNumber,
                            DocumentType = c.DocumenIdentificationTypeId,
                            Email = c.Email,
                            Name = $"{c.Name}{c.LastName}",
                            PaymentOcurrencyId = m.PaymentOcurrencyId,
                            Phone = c.PhoneNumber,
                            MedPersons = Context.MedPersonInsureds.Where(p => p.MedEnsuraceApplicationId == m.Id).Select(p => new CreateMedGEADto
                            {
                                DocumentNumber = p.DocumentNumber,
                                DocumentType = p.DocumentTypeId
                            }).ToList(),
                        };
            return await query.FirstOrDefaultAsync();
        }
    }
}
