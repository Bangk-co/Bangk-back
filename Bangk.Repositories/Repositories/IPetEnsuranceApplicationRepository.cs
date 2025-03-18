using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface IPetEnsuranceApplicationRepository : IGenericRepository<PetEnsuranceApplication, PetEnsuranceDto>
    {
        Task<PetEnsuranceApplication> GetPetApplicationByReference(string reference);
    }
}
