using System;

namespace Bangk.Repositories.Entities
{
    public class Notification : EntityBase
    {
        public int ApplicationId { get; set; }
        public DateTime DateToSendNotification { get; set; }
        public int NotificationType { get; set; }
    }
}
