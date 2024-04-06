using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class BeneficiaryDto
    {
        public int LoanInsurancePolicyId { get; set; }
        public bool? Beneficiaries { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int IdentificationTypeId { get; set; }
        public string IdentificationNumber { get; set; }
        public int RelationshipId { get; set; }
        public decimal PercentInherit { get; set; }
    }
}
