using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class LoanInsurancePolicyDto
    {
        public int? LoanApplicationId { get; set; }
        public int? HousingTypeId { get; set; }
        public long? HousingValue { get; set; }
        public DateTime? BuiltgDate { get; set; }
        public int? BuiltArea { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public int? BirthCityId { get; set; }
        public int? CountryNationalityId { get; set; }
        public int? NumberChildren { get; set; }
        public int? CivilStatusId { get; set; }
        public string Occupation { get; set; }
        public int? OccupationActivityId { get; set; }
        public bool? IsPublicPerson { get; set; }
        public int? ResidenceCityId { get; set; }
        public long? MonthlyIncome { get; set; }
        public long? MonthlyExpenses { get; set; }
        public long? AssetValue { get; set; }
        public long? LiabilitiesValue { get; set; }
        public int? Weight { get; set; }
        public int? Height { get; set; }
        public bool? HIVActive { get; set; }
        public bool? PlannedMedicalProcidure { get; set; }
        public bool? Disability { get; set; }
        public bool? LifeEnsure { get; set; }
        public bool? Convicted { get; set; }
        public bool? Beneficiaries { get; set; }
        public string IncomeOrigin { get; set; }
        public int? BasementNumber { get; set; }
        public int? ApartmentNumber { get; set; }
        public int? BuildingFloorsNumber { get; set; }
        public int DocumentIdentificationTypeId { get; set; }
        public string IdentificationNumber { get; set; }
        public DateTime? DocumentIssueDate { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int CountryCodePhoneId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set;}
        public string Diseases { get; set; }
        public string DiseasesMoreInfo { get; set; }
        public string PlannedMedicalProcidureMoreInfo { get; set; }
        public string DisabilityMoreInfo { get; set; }
        public string BeneficiaryName { get; set; }
        public int? BeneficiaryIdentificationTypeId { get; set; }
        public string BeneficiaryIdentificationNumber { get; set; }
        public int ClienId { get; set; }
        public DateTime BirthDate { get; set; }
        public int GenderId { get; set; }
        public long LoanBalance { get; set; }
        public long? MonthlyCostInsurance { get; set; }
        public int? BirthStateId { get; set; }
        public int? BirthCountryId { get; set; }
        public int? ResidenceCountryId { get; set; }
        public int? ResidenceStateId { get; set; }
        public int? StateId { get; set; }
        public int LoanTypeId { get; set; }
        public int? CityVehiculeId { get; set; }
        public string VehicleLicensePlate { get; set; }
        public int? StateVehiculeId { get; set; }
        public int? CountryVehiculeId { get; set; }
    }
}
