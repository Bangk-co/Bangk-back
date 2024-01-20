using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Entities
{
    public class LoanApplication : EntityBase
    {
        [ForeignKey("Client")]
        public int ClientId { get; set; }
        public decimal LoanBalance { get; set; }

        [ForeignKey("Setting")]
        public int BankId { get; set; }

        [ForeignKey("Setting")]
        public int LoanType { get; set; }
        public string LoanNumber { get; set; }

        [ForeignKey("User")]
        public int ReviewerId { get; set; }

        [ForeignKey("Setting")]
        public int StateId { get; set; }

        public Client Clients { get; set; }
        public Setting Banks { get; set; }
        public Setting LoanTypes { get; set; }
        public Setting States { get; set; }
        public User User { get; set; }
        

    }
}
