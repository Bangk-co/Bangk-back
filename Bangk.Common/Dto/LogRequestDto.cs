using System;

namespace Bangk.Common.Dto
{
    public class LogRequestDto
    {
        public DateTime FromDate { get; set; } = DateTime.Today.AddDays(-10);
        public DateTime ToDate { get; set; } = DateTime.Today.AddDays(10);
        public int RegisterCount { get; set; } = 100;
    }
}
