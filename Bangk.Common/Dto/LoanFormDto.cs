using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class LoanFormDto
    {
        public int LoanApplicationId { get; set; }
        public string FormName { get; set; }
        public string Concept { get; set; }
        public string Comment { get; set; }
        
    }
}
