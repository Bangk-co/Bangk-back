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
        public int LoanBalance { get; set; }

        [ForeignKey("Setting")]
        public int BankId { get; set; }  
        public int LoanType { get; set; }
        public string LoanNumber { get; set; }

        [ForeignKey("User")]
        public int Reviewerld { get; set; }

        [ForeignKey("Setting")]
        public int Stateld { get; set; }
        

    }
}
