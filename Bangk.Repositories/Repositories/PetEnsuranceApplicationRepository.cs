using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;

namespace Bangk.Repositories.Repositories
{
    public class PetEnsuranceApplicationRepository : GenericRepository<PetEnsuranceApplication, PetEnsuranceDto>, IPetEnsuranceApplicationRepository
    {
        public PetEnsuranceApplicationRepository(AppDbContext context) : base(context)
        {
        }
    }
}
