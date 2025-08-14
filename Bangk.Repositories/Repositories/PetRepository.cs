using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;

namespace Bangk.Repositories.Repositories
{
    public class PetRepository : GenericRepository<Pet, PetsDto>, IPetRepository
    {
        public PetRepository(AppDbContext context) : base(context)
        {
        }
    }
}