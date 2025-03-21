using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class MEdEnsuranceApplicationRepository: GenericRepository<MedInsuranceApplication, MedEnsuranceApplicationDto>, IMedEnsuranceApplicationRepository
    {
        public MEdEnsuranceApplicationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<MedInsuranceApplication> GetMedEnsuranceApplicationByReference(string reference)
        {
            return await Context.MedInsuranceApplication.FirstOrDefaultAsync(p => p.TransactionReference == reference);
        }
    }
}
