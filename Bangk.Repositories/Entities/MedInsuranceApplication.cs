using System.ComponentModel.DataAnnotations.Schema;

namespace Bangk.Repositories.Entities
{
    public class MedInsuranceApplication : EntityBase
    {
        [ForeignKey("Setting")]
        public int PaymentOcurrencyId { get; set; }
        public bool Deleted { get; set; }
        public int Status { get; set; }
        public string TransactionReference { get; set; }
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        [ForeignKey("User")]
        public int ReviewerId { get; set; }
        public string TransactionId { get; set; }
        public string TokenPaymentSource { get; set; }
    }
}
