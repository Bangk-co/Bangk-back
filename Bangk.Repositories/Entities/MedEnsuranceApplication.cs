using System.ComponentModel.DataAnnotations.Schema;

namespace Bangk.Repositories.Entities
{
    public class MedEnsuranceApplication : EntityBase
    {
        [ForeignKey("Setting")]
        public int PaymentOcurrencyId { get; set; }
        public bool Deleted { get; set; }
        public int Status { get; set; }
        public string TransactionReference { get; set; }
        public long? PaymentSourceId { get; set; }
        [ForeignKey("Client")]
        public int ClientId { get; set; }

    }
}
