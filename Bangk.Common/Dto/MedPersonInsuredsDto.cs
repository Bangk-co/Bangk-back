using System;
using System.Collections.Generic;

namespace Bangk.Common.Dto
{
    public class MedPersonInsuredsDto
    {
        public int PaymentOcurrencyId { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentNumber { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime TreatPErsonalDatadate { get; set; }
        public int CountryCodePhoneId { get; set; }
        public bool IsBeneficiary { get; set; }
        public List<BeneficiaryInsuranceDto> BeneficiaryInsurance { get; set; }
    }

    public class BeneficiaryInsuranceDto
    {
        public int DocumentTypeId { get; set; }
        public string DocumentNumber { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime TreatPErsonalDatadate { get; set; }
        public int CountryCodePhoneId { get; set; }
    }
}
