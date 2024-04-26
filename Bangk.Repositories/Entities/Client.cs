using System;
using System.Collections.Generic;

namespace Bangk.Repositories.Entities
{
    public class Client : EntityBase
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int? CountryCodePhoneId { get; set; }
        public string Email { get; set; }
        public string IdentificationNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public int DocumenIdentificationTypeId { get; set; }
        public int? GenderId { get; set; }
        public DateTime PermissionTreatPersonalDataDate { get; set; }
        public virtual ICollection<LoanApplication> LoanApplications { get; set; }
        public DateTime? DocumentIssueDate {  get; set; }
        public DateTime? ValidateClientOtp { get; set; }

        public int State {  get; set; }
    }
}
