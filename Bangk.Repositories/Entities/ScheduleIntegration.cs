using System;

namespace Bangk.Repositories.Entities
{
    public class ScheduleIntegration : EntityBase
    {
        public int IntegrationType { get; set; }
        public int CountFails { get; set; }
        public DateTime NextTryDate { get; set; }
        public int  LoanApplicationId { get; set; }
    }
}
