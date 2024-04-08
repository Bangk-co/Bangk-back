using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;

namespace Bangk.Repositories.Repositories
{
    public class ValidationSBSLogRepository : GenericRepository<ValidationSBSLog, ValidationSBSLogDto>, IValidationSBSLogRepository
    {
        public ValidationSBSLogRepository(AppDbContext context) : base(context)
        {

        }
    }
}
