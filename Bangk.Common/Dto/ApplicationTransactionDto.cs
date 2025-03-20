using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class ApplicationTransactionDto
    {
        public int ApplicationType { get; set; }
        public string TransactionNumber { get; set; }
        public int Status { get; set; }
        public bool Deleted { get; set; }
    }
}
