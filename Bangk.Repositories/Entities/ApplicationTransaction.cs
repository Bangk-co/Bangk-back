using System;

namespace Bangk.Repositories.Entities
{
    public class ApplicationTransaction: EntityBase
    {
        public string TransactionNumber { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public string Currency { get; set; }
        public DateTime TransactionCreationDate { get; set; }
        public long Amount { get; set; }
        public string PaymentMethodType { get; set; }
        public int? PaymentInstallments { get; set; }
    }
}
