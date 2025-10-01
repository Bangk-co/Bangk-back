using Bangk.Common.Enums;

namespace Bangk.Common.Dto
{
    public class UserPaymentRequest
    {
        public string Email { get; set; }
        public int ApplicationId { get; set; }  
        public int PaymentSourceId { get; set; }
        public long Amount { get; set; }
        public string TypeApplication {  get; set; }
        public string TokenPayment { get; set; }
    }
}
