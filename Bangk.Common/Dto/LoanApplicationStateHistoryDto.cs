using System;
using System.Collections.Generic;
using System.Linq;

namespace Bangk.Common.Dto
{

    public class LoanApplicationStateHistoryResultDto
    {
        public IQueryable<LoanApplicationStateHistoryDto> LoanApplicationHistory {  get; set; }
        public List<string> States { get; set; }
    }


    public class LoanApplicationStateHistoryDto
    {
        public int Id { get; set; }
        public string Observations { get; set; }
        public string StateName { get; set; }
        public int LoanApplicationId { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
