using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;

namespace Bangk.Common.Dto
{
    public class ClientDto
    {
        public int IdClient { get; set; }
        public string Name { get; set; }

        public DateTime CreationDate { get; set; }

        public int Applications { get; set; }

        public decimal LoanBalance { get; set; }

        public decimal Sales { get; set; }

        public string ReviewerName { get; set; }

        public string StateName { get; set; }


    }
}
