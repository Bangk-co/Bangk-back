using Microsoft.VisualBasic;
using System;

namespace Bangk.Common.Dto
{
    public class ClientDto
    {
        public string Name { get; set; }

        public DateTime CreationDate { get; set; }

        public int Applications { get; set; }

        public decimal LoanBalance { get; set; }

        public decimal Sales { get; set; }

        public string ReviewerName { get; set; }

        public string StateName { get; set; }

    }
}
