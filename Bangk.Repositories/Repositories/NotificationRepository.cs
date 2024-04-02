using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class NotificationRepository : GenericRepository<Notification, NotificationDto>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<List<Notification>> GetNotificationByApplicationId(int applicationId)
        {
            return await Context.Notifications.Where(n => n.ApplicationId == applicationId).ToListAsync();
        }
    }
}
