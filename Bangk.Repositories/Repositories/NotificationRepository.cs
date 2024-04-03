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

        public async Task<List<NotificationDto>> GetNotificationByDate(DateTime dateToSent)
        {
            var query = from n in Context.Notifications
                        join l in Context.LoanApplication on n.ApplicationId equals l.Id
                        join c in Context.Clients on l.ClientId equals c.Id
                        where n.DateToSendNotification >= dateToSent.AddMinutes(-5) && n.DateToSendNotification <= dateToSent
                        select new NotificationDto
                        {
                            ApplicationId = l.Id,
                            ClientLastName = c.LastName,
                            ClientName = c.Name,
                            DateToSendNotification = n.DateToSendNotification,
                            NotificationType = n.NotificationType,
                            ClientEmail = c.Email
                        };
            return await query.ToListAsync();
        }
    }
}
