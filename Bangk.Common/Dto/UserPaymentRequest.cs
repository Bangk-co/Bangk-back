using Bangk.Common.Enums;

namespace Bangk.Common.Dto
{
    public class UserPaymentRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public int ApplicationId { get; set; }  
        public DocumentType DocumentIdentificationType { get; set; }
        public string DocumentIdentificationNumber { get; set; }
        public int PaymentSourceId { get; set; }
        public int Amount { get; set; }
        public string TypeApplication {  get; set; }
        public MethodPaymentTimeEnum TimeApplication { get; set; }
        public string TokenPayment { get; set; }
    }
}
