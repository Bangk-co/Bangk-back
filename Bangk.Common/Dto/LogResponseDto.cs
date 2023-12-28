using System;

namespace Bangk.Common.Dto
{
    public class LogResponseDto
    {
        public string Level { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Exception { get; set; }
    }
}
