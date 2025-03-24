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
    }
}
