namespace Bangk.Common.Dto
{
    public class PaymentCardResponseDto
    {
        public string TransactionReference { get; set; }
        public long Amount { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
    }
}
