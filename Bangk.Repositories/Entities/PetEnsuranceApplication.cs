using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bangk.Repositories.Entities
{
    public class PetInsuranceApplication : EntityBase
    {
        [ForeignKey("Setting")]
        public int PaymentOcurrencyId {  get; set; }
        public int Status { get; set; }
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public string TransactionReference { get; set; }
        [ForeignKey("User")]
        public int? ReviewerId { get; set; }

        public string TransactionId { get; set; }
        public int? TokenPaymentSource { get; set; }
    }
}
