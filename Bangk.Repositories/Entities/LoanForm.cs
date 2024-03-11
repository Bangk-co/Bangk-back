using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Entities
{
    public class LoanForm : EntityBase
    {
        public string FormName { get; set; }
        public string Concept {  get; set; }
        public string Comment { get; set; }

        [ForeignKey("LoamApplication")]
        public int LoanApplicationId { get; set; }
    }
}
