using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class PetEnsuranceApplicationRepository : GenericRepository<PetEnsuranceApplication, PetEnsuranceDto>, IPetEnsuranceApplicationRepository
    {
        public PetEnsuranceApplicationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PetEnsuranceApplication> GetPetApplicationByReference(string reference)
        {
            return await Context.PetEnsuranceApplication.FirstOrDefaultAsync(p => p.TransactionReference == reference);
        }
    }
}
