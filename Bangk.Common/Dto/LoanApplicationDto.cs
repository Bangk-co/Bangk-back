using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class LoanApplicationDto
    {
        
        public int ClientId { get; set; }
        public int LoanBalance { get; set; }
        public int BankId { get; set; }
        public int LoanType { get; set; }
        public string LoanNumber { get; set; }
        public int Reviewerld { get; set; }
        public int Stateld { get; set; }
    }
}
