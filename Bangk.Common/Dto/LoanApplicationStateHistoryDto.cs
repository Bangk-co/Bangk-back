using System;

namespace Bangk.Common.Dto
{
    public class LoanApplicationStateHistoryDto
    {
        public int Id { get; set; }
        public string Observations { get; set; }
        public int StateId { get; set; }
        public int LoanApplicationId { get; set; }
    }
}
