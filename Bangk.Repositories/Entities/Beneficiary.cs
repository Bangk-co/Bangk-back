using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Entities
{
    public class Beneficiary: EntityBase
    {
        [ForeignKey("LoanApplication")]
        public int LoanInsurancePolicyId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }

        [ForeignKey("Setting")]
        public int IdentificationTypeId { get; set; }
        public string IdentificationNumber { get; set; }

        [ForeignKey("Setting1")]
        public int RelationshipId { get; set; }
        public decimal PercentInherit { get; set; }
        public LoanApplication LoanApplication { get; set; }
        public Setting Setting { get; set; }

        public Setting Setting1 { get; set; }

    }
}
