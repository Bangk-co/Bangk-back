using System;
using System.Collections.Generic;

namespace Bangk.Common.Dto
{
    public class LoanInsurancePolicySaveDto
    {
        public int LoanApplicationId { get; set; }
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
        public bool? HasBondPublicPerson { get; set; }
        public int? ResidenceCityId { get; set; }
        public long? MonthlyIncome { get; set; }
        public long? MonthlyExpenses { get; set; }
        public long? AssetValue { get; set; }
        public long? LiabilitiesValue { get; set; }
        public DateTime? AcceptanceOriginOfFunds { get; set; }
        public int? Weight { get; set; }
        public int? Height { get; set; }
        public bool? HIVActive { get; set; }
        public bool? PlannedMedicalProcidure { get; set; }
        public bool? Disability { get; set; }
        public bool? LifeEnsure { get; set; }
        public bool? Convicted { get; set; }
        public string IncomeOrigin { get; set; }
        public int? BasementNumber { get; set; }
        public bool? Beneficiaries { get; set; }
        public int? ApartmentNumber { get; set; }
        public int? BuildingFloorsNumber { get; set; }
        public string Diseases { get; set; }
        public string DiseasesMoreInfo { get; set; }
        public string PlannedMedicalProcidureMoreInfo { get; set; }
        public string DisabilityMoreInfo { get; set; }
        public int? CityVehiculeId { get; set; }
        public string VehicleLicensePlate { get; set; }
        public DateTime? OtpDateForms { get; set; }



        public List<BeneficiarySaveDto> beneficiary { get; set;}


    }

    public class BeneficiarySaveDto
    {
        public int Id { get; set; }
        public int LoanInsurancePolicyId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int IdentificationTypeId { get; set; }
        public string IdentificationNumber { get; set; }
        public int RelationshipId { get; set; }
        public decimal PercentInherit { get; set; }
        public DateTime CreationDate { get; set; }



    }
}
