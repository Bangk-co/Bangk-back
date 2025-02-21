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
        public PublicData PublicData { get; set; }
        public string Token { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string CustomerEmail { get; set; }
    }

    public class PublicData
    {
        public string Type { get; set; }
        public string PaymentDescription { get; set; }
        public string BankAccountType { get; set; }
        public string BankAccountLastFour { get; set; }
    }

    public class Meta
    {

    }
}
