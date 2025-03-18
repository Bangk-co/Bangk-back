using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Entities
{
    public class MedEnsuranceApplication : EntityBase
    {
        [ForeignKey("Setting")]
        public int MethodPaymentId { get; set; }
        public bool Deleted { get; set; }
        public int Status { get; set; }
        public string TransactionReference { get; set; }
        public long? PaymentSourceId { get; set; }
    }
}
