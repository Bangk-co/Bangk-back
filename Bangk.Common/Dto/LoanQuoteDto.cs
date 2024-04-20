using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class LoanQuoteDto
    {
        public int InsuranceId { get; set; }
        public int LoanApplicationId { get; set; }
        public long YearlyFee { get; set;}
        public DateTime? TermsConditionsAcceptanceQuote { get; set; }

    }
}
