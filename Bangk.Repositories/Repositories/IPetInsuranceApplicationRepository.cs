using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface IPetInsuranceApplicationRepository : IGenericRepository<PetInsuranceApplication, PetInsuranceDto>
    {
        Task<PetInsuranceApplication> GetPetApplicationByReference(string reference);
    }
}
