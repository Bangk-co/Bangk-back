using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bangk.Repositories.Entities
{
    public class LoanApplication : EntityBase
    {
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public long LoanBalance { get; set; }

        [ForeignKey("Setting")]
        public int BankId { get; set; }

        [ForeignKey("Setting")]
        public int LoanTypeId { get; set; }
        public string LoanNumber { get; set; }
        [ForeignKey("User")]
        public int ReviewerId { get; set; }
        
        public long? MonthlyCostInsurance { get; set; }

        public Client Client { get; set; }
        public Setting Bank { get; set; }
        public Setting LoanType { get; set; }
        public User User { get; set; }
        public virtual ICollection<LoanApplicationDocument> LoanApplicationDocuments { get; set; }
    }
}
