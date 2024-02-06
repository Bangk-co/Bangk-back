using System.ComponentModel.DataAnnotations.Schema;

namespace Bangk.Repositories.Entities
{
    public class LoanApplicationStateHistory : EntityBase
    {
        public string Observations { get; set; }
        [ForeignKey("LoanApplication")]
        public int LoanApplicationId { get; set; }
        [ForeignKey("Setting")]
        public int StateId { get; set; }

        public LoanApplication LoanApplication { get; set; }
        public Setting State { get; set; }
        
    }
}
