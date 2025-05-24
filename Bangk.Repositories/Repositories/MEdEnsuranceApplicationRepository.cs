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
                            Name = $"{c.Name} {c.LastName}",
                            PaymentOcurrencyId = m.PaymentOcurrencyId,
                            DocumentNumber = c.IdentificationNumber,
                            MedPersons = Context.MedPersonInsureds.Where(p => p.MedEnsuraceApplicationId == m.Id).Select(p => new CreateMedGEADto
                            {
                                DocumentNumber = p.DocumentNumber,
                                DocumentType = p.DocumentTypeId,
                                Name = $"{p.Name} {p.LastName}",
                                Email = p.Email,
                                PhoneNumber = p.PhoneNumber,
                            }).ToList(),
                        };
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<PaymentJobDto>> GetMedAssistencePendingToPay(DateTime dateTime)
        {
            var query = from p in Context.MedInsuranceApplication
                        join c in Context.Clients on p.ClientId equals c.Id
                        where p.NextPaymentDate.Value.Date == dateTime.Date && p.Status == (int)InsuranceStatesEnum.Approve
                        select new PaymentJobDto
                        {
                            ApplicationId = p.Id,
                            ClientId = p.ClientId,
                            ClientName = $"{c.Name} {c.LastName}",
                            NextPaymentDate = p.NextPaymentDate,
                            TokenPaymentSource = p.TokenPaymentSource,
                            TransactionReference = p.TransactionReference,
                            Amount = p.Amount,
                            PaymentInstallments = p.PaymentInstallments,
                            Email = c.Email,
                        };

            return await query.ToListAsync();
        }

        public async Task<List<PaymentJobDto>> GetMedAssistencePayment(string applicationReference)
        {
            var query = from p in Context.MedInsuranceApplication
                        join c in Context.Clients on p.ClientId equals c.Id
                        where p.TransactionReference == applicationReference
                        select new PaymentJobDto
                        {
                            ApplicationId = p.Id,
                            ClientId = p.ClientId,
                            ClientName = $"{c.Name} {c.LastName}",
                            NextPaymentDate = p.NextPaymentDate,
                            TokenPaymentSource = p.TokenPaymentSource,
                            TransactionReference = p.TransactionReference,
                            Amount = p.Amount,
                            PaymentInstallments = p.PaymentInstallments,
                            Email = c.Email,
                        };

            return await query.ToListAsync();
        }
    }
}
