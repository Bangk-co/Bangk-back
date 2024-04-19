using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Entities
{
    public class LoanQuote : EntityBase
    {
        [ForeignKey("Setting")]
        public int InsuranceId { get; set; }

        [ForeignKey("LoanApplication")]
        public int LoanApplicationId { get; set; }
        public decimal YearlyFee { get; set; }
        public DateTime? TermsConditionsAcceptanceQuote { get; set; }
        public DateTime? AcceptQuoteOtp { get; set; }
        public Setting Insurance { get; set; }
        public LoanApplication LoanApplication { get; set; }


    }
}
