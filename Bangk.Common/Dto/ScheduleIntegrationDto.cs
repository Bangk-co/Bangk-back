using System;

namespace Bangk.Common.Dto
{
    public class ScheduleIntegrationDto
    {
        public int IntegrationType { get; set; }
        public int CountFails { get; set; }
        public DateTime NextTryDate { get; set; }
        public int LoanApplicationId { get; set; }
    }
}
