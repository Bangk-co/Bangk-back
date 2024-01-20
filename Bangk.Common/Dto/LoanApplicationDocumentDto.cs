using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class LoanApplicationDocumentDto
    {
        public int LoanApplicationId { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public int DocumentType { get; set; }
        public string Url { get; set; }

    }
}
