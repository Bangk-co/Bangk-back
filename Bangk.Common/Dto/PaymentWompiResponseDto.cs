using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class PaymentWompiResponseDto
    {
        public Data Data { get; set; }
        public Meta Meta { get; set; }
    }

    public class Data
    {
        public int Id { get; set; }
        public PublicData Public_Data { get; set; }
        public string Token { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Customer_Email { get; set; }
    }

    public class PublicData
    {
        public string Type { get; set; }
        public string Payment_Description { get; set; }
        public string Bank_Account_Type { get; set; }
        public string Bank_Account_Last_Four { get; set; }
    }

    public class Meta
    {

    }
}