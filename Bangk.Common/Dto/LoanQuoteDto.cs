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
        public decimal YearlyFee { get; set;}
        public DateTime? TermsConditionsAcceptanceQuote { get; set; }
        public DateTime? AcceptQuoteOtp { get; set; }
        public DateTime? SbsPrivacyPolicies { get; set; }


    }
}
