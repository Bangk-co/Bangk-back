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
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime CreationDatePolicy { get; set;}
        public string NameBank { get; set; }
        public string NitBank { get; set; }
        public long LoanBalance { get; set; }
        public decimal YearlyFee { get; set; }
        public List<BeneficiariesJobDto> Beneficiary { get; set; }

    }

    public class ClientReport2Dto
    {
        public DateTime PermissionTreatPersonalDataDate { get; set; }
        public DateTime? TermsConditionsAcceptanceQuote {  get; set; }
        public DateTime? ValidateClientOtp { get; set; }
        public DateTime? AcceptQuoteOtp { get; set; }
        public DateTime? AcceptanceOriginOfFunds { get; set; }
        public DateTime? OtpDateForms { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string DocumenIdentificationTypeId { get; set; }
        public string IdentificationNumber { get; set; }
        public string CountryCodePhoneId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string CountryNationality { get; set; }
        public string CivilStatus { get; set; }
        public int? NumberChildren { get; set; }
        public string Occupation { get; set; }
        public string OccupationActivity { get; set; }
        public bool? IsPublicPerson { get; set; }
        public bool? HasBondPublicPerson { get; set; }
        public string ResidenceCountry { get; set; }
        public string ResidenceCity { get; set; }
        public string ResidenceState { get; set; }
        public long? MonthlyIncome { get; set; }
        public long? MonthlyExpenses { get; set; }
        public long? AssetValue { get; set; }
        public long? LiabilitiesValue { get; set; }
        public string IncomeOrigin { get; set; }
        public int? Weight { get; set; }
        public int? Height { get; set; }
        public string Diseases { get; set; }
        public string DiseasesMoreInfo { get; set; }
        public bool? HIVActive { get; set; }
        public bool? PlannedMedicalProcidure { get; set; }
        public string PlannedMedicalProcidureMoreInfo { get; set; }
        public bool? Disability { get; set; }
        public string DisabilityMoreInfo { get; set; }
        public bool? Convicted { get; set; }
        public bool? LifeEnsure { get; set; }
        public string NameBank { get; set; }
        public string NitBank { get; set; }
        public List<Beneficiaries2JobDto> Beneficiary { get; set; }

    }

    public class BeneficiariesJobDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int RelationshipId { get; set; }
        public decimal PercentInherit { get; set; }
    }
    public class Beneficiaries2JobDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string DocumenIdentificationTypeB {  get; set; }
        public string IdentificationNumberB { get; set; }
        public string Relationship { get; set; }
        public decimal PercentInherit { get; set; }
    }

}
