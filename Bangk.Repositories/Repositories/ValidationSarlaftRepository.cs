using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;

namespace Bangk.Repositories.Repositories
{
    public class ValidationSarlaftRepository : GenericRepository<ValidationSarlaftLog, ValidationSarlaftLogDto>, IValidationSarlaftRepository
    {
        public ValidationSarlaftRepository(AppDbContext context) : base(context)
        {

        }
    }
}
