using System;

namespace Bangk.Common.Dto
{
    public class PaymentJobDto
    {
        public DateTime? NextPaymentDate { get; set; }
        public int? TokenPaymentSource { get; set; }
        public string TransactionReference { get; set; }
        public int ClientId { get; set; }
        public long? Amount { get; set; }
        public int? PaymentInstallments { get; set; }
        public string Email { get; set; }
        public int ApplicationId { get; set; }
        public string ClientName { get; set; }
    }
}
