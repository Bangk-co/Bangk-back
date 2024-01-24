using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Entities
{
    public class LoanApplicationDocument : EntityBase
    {
        [ForeignKey("LoanApplication")]
        public int LoanApplicationId { get; set; }
        public string Name { get; set; }
        public int DocumentType { get; set; }
        public string Url { get; set; }

        public LoanApplication LoanApplication { get; set; }



    }
}

