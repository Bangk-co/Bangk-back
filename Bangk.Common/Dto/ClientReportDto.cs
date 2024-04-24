using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class ClientReportDto
    {
        public int DocumenIdentificationTypeId { get; set; }
        public string IdentificationNumber { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public int? City { get; set; }
        public int State { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime CreationDatePolicy { get; set;}
        public string NameBank { get; set; }
        public string NitBank { get; set; }
        public long LoanBalance { get; set; }
        public decimal YearlyFee { get; set; }
        public List<BeneficiariesJobDto> Beneficiary { get; set; }

    }

    public class BeneficiariesJobDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int RelationshipId { get; set; }
        public decimal PercentInherit { get; set; }
    }



}
