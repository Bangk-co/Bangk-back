using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public DateTime DateToSendNotification { get; set; }
    }
}
