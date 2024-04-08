using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;

namespace Bangk.Repositories.Repositories
{
    public class ScheduleIntegrationRepository : GenericRepository<ScheduleIntegration, ScheduleIntegrationDto>, IScheduleIntegrationRepository
    {
        public ScheduleIntegrationRepository(AppDbContext context) : base(context)
        {

        }
    }
}
