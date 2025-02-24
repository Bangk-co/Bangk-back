using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class MEdEnsuranceApplicationRepository: GenericRepository<MedEnsuranceApplication, MedEnsuranceApplicationDto>, IMedEnsuranceApplicationRepository
    {
        public MEdEnsuranceApplicationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<MedEnsuranceApplication> GetMedEnsuranceApplicationByReference(string reference)
        {
            return await Context.MedEnsuranceApplication.FirstOrDefaultAsync(p => p.TransactionReference == reference);
        }
    }
}
