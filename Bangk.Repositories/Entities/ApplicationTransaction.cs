using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Entities
{
    public class ApplicationTransaction: EntityBase
    {
        public int ApplicationType { get; set; }
        public string TransactionNumber { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public string Currency { get; set; }
        public DateTime TransactionCreationDate { get; set; }
        public long Amount { get; set; }
        public string PaymentMethodType { get; set; }
    }
}
