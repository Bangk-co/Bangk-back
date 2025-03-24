using System;

namespace Bangk.Common.Dto
{
    public class StatusTransactionResponseDto
    {
        public DataStatusDto Data { get; set; }
    }

    public class DataStatusDto
    {
        public string Reference { get; set; }
        public string Status { get; set; }
        public int Amount_In_Cents { get; set; }
        public string Currency { get; set; }
        public string Payment_Method_Type { get; set; }
        public DateTime Created_At { get; set; }
        public string Id { get; set; }
        
    }
}
