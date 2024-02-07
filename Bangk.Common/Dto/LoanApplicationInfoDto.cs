using System;

namespace Bangk.Common.Dto
{
    public class LoanApplicationInfoDto
    {
        public string ClientName { get; set; }
        public string ClientLastName { get; set; }
        public int DocumenIdentificationTypeId { get; set; }
        public string IdentificationNumber { get; set; }
        public string Email {  get; set; }
        public int CountryCodePhoneId { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public int GenderId { get; set; }
        public long LoanBalance { get; set; }
        public int BankId { get; set; }
        public int LoanType { get; set; }
        public long LoanNumber { get; set; }
        public long? MonthlyCostInsurance { get; set; }
        public string Observations { get; set; }
        public int ClientId { get; set; }
        public int LoanApplicationId { get; set; }
    }
}
