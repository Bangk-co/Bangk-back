using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Entities
{
    public class TryPaymentJob: EntityBase
    {
        public string ApplicationReference { get; set; }
        public int CountFailed { get; set; }
    }
}
