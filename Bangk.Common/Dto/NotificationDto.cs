using System;

namespace Bangk.Common.Dto
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public DateTime DateToSendNotification { get; set; }
        public string ClientName { get; set; }
        public string ClientLastName {  get; set; }
        public string ClientEmail { get; set; }
        public int NotificationType { get; set; }
    }
}
