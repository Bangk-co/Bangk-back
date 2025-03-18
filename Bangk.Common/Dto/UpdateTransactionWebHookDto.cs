using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class UpdateTransactionWebHookDto
    {
        public string Event { get; set; }
        public TransactionDataDto Data { get; set; }
        public DateTime Sent_At { get; set; }
    }

    public class TransactionDataDto
    {
        public TransactionDto Transaction { get; set; }
    }
    public class TransactionDto
    {
        public string Id { get; set; }
        public int Amount_In_Cents { get; set; }
        public string Reference { get; set; }
        public string Customer_Email { get; set; }
        public string Currency { get; set; }
        public string Payment_Method_Type { get; set; }
        public string Redirect_Url { get; set; }
        public string Status { get; set; }
        public string Shipping_Address { get; set; }
        public string Payment_Link_Id { get; set; }
        public string Payment_Source_Id { get; set; }
    }
}
