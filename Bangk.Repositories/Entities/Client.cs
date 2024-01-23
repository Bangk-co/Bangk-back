using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bangk.Repositories.Entities
{
    public class Client : EntityBase
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int CountryCodePhoneId { get; set; }
        public string Email { get; set; }
        public string IdentificationNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string IdentificationFileUrl { get; set; }
        public int DocumenIdentificationTypeId { get; set; }
        public int GenderId { get; set; }

        public virtual ICollection<LoanApplication> LoanApplications { get; set; }
    }
}
