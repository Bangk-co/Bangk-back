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
        
        public string ClientName { get; set; }
        public decimal LoanBalance { get; set; }
        public string BankName { get; set; }
        public string LoanTypeName { get; set; }
        public int LoanNumber { get; set; }
        public string ReviewerName { get; set; }
        public string StateName { get; set; }
    }
}
