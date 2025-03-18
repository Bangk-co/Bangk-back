using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
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
    }
}
