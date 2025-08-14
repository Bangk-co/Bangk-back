using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class CardTokenResponseDto
    {
        public string Status { get; set; }
        public DataDto Data { get; set; }
    }

    public class DataDto
    {
        public string Id { get; set; }
        public string Created_At { get; set; }
        public string Brand { get; set; }
        public string Name { get; set; }
        public string Last_Four { get; set; }
        public string Bin { get; set; }
        public string Exp_Year { get; set; }
        public string Exp_Month { get; set; }
        public string Card_Holder { get; set; }
        public string Expires_At { get; set; }
    }

    public class CardPaymentSourceDto
    {
        public DataPaymentSourceDto Data { get; set; }
    }

    public class DataPaymentSourceDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}