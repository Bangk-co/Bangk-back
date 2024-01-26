using Microsoft.AspNetCore.Http;
using System;

namespace Bangk.Common.Dto
{
    public class CreateClientDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int CountryCodePhoneId { get; set; }
        public string Email { get; set; }
        public string IdentificationNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public IFormFile IdentificationFile { get; set; }
        public int DocumenIdentificationTypeId { get; set; }
        public int GenderId { get; set; }
        public DateTime PermissionTreatPersonalDataDate { get; set; }
        public CreateLoadAplicacionDto LoadAplicacionDto { get; set; }
    }

    public class CreateLoadAplicacionDto
    {
        public IFormFile BankStatementFile { get; set; }
        public int BankId { get; set; }
        public int LoanTypeId { get; set; }
        public long LoanBalance { get; set; }
        public long LoanNumber { get; set; }

    }
}
