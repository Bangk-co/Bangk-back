using System;
using System.Collections.Generic;

namespace Bangk.Common.Dto
{
    public class PetInsuranceDto
    {
        public int PaymentOcurrencyId { get; set; }
        public int DocumentTypeId { get; set; }
        public string DocumentNumber { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime TreatPersonalDataDate { get; set; }
        public List<PetsDto> Pets { get; set; }
        public int? CountryCodePhoneId { get; set; }
        public long Amount { get; set; }
    }

    public class PetsDto
    {
        public int PetTypeId { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
