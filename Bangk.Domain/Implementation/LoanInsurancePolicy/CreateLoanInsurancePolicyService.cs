using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanInsurancePolicy;
using Bangk.Repositories.Repositories;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Client
{
    public class CreateLoanInsurancePolicyService : ICreateLoanInsurancePolicyService
    {
        private readonly ILoanInsurancePolicyRepository _loanInsurancePolicyRepository;


        public CreateLoanInsurancePolicyService(ILoanInsurancePolicyRepository loanInsurancePolicyRepository)
        {
            _loanInsurancePolicyRepository = loanInsurancePolicyRepository;
        }

        public async Task<ResponseDto<LoanInsurancePolicyDto>> Execute(LoanInsurancePolicySaveDto loanInsurancePolicySaveDto)
        {
            var result = new ResponseDto<LoanInsurancePolicyDto>
            {
                Result = new LoanInsurancePolicyDto()
            };

            var loanInsurencePolicy = await _loanInsurancePolicyRepository.GetLoanInsurancePolicyByLoadApplicationId(loanInsurancePolicySaveDto.LoanApplicationId);
            if (loanInsurencePolicy == null)
            {
                var newLoanInsurencePolicy = new Repositories.Entities.LoanInsurancePolicy
                {
                    Address = loanInsurancePolicySaveDto.Address,
                    IsPublicPerson = loanInsurancePolicySaveDto.IsPublicPerson,
                    LiabilitiesValue = loanInsurancePolicySaveDto.LiabilitiesValue,
                    HousingValue = loanInsurancePolicySaveDto.HousingValue,
                    HousingTypeId = loanInsurancePolicySaveDto.HousingTypeId,
                    HIVActive = loanInsurancePolicySaveDto.HIVActive,
                    Height = loanInsurancePolicySaveDto.Height,
                    AssetValue = loanInsurancePolicySaveDto.AssetValue,
                    Beneficiaries = loanInsurancePolicySaveDto.Beneficiaries,
                    BirthCityId = loanInsurancePolicySaveDto.BirthCityId,
                    BuiltArea = loanInsurancePolicySaveDto.BuiltArea,
                    BuiltgDate = loanInsurancePolicySaveDto.BuiltgDate,
                    CityId = loanInsurancePolicySaveDto.CityId,
                    CivilStatusId = loanInsurancePolicySaveDto.CivilStatusId,
                    Convicted = loanInsurancePolicySaveDto.Convicted,
                    CountryNationalityId = loanInsurancePolicySaveDto.CountryNationalityId,
                    Disability = loanInsurancePolicySaveDto.Disability,
                    LifeEnsure = loanInsurancePolicySaveDto.LifeEnsure,
                    MonthlyExpenses = loanInsurancePolicySaveDto.MonthlyExpenses,
                    MonthlyIncome = loanInsurancePolicySaveDto.MonthlyIncome,
                    NumberChildren = loanInsurancePolicySaveDto.NumberChildren,
                    Occupation = loanInsurancePolicySaveDto.Occupation,
                    OccupationActivityId = loanInsurancePolicySaveDto.OccupationActivityId,
                    PlannedMedicalProcidure = loanInsurancePolicySaveDto.PlannedMedicalProcidure,
                    ResidenceCityId = loanInsurancePolicySaveDto.ResidenceCityId,
                    Weight = loanInsurancePolicySaveDto.Weight,
                    LoanApplicationId = loanInsurancePolicySaveDto.LoanApplicationId,
                    ApartmentNumber = loanInsurancePolicySaveDto.ApartmentNumber,
                    BasementNumber = loanInsurancePolicySaveDto.BasementNumber,
                    IncomeOrigin = loanInsurancePolicySaveDto.IncomeOrigin,
                    BuildingFloorsNumber = loanInsurancePolicySaveDto.BuildingFloorsNumber,
                    BeneficiaryIdentificationNumber = loanInsurancePolicySaveDto.BeneficiaryIdentificationNumber,
                    BeneficiaryIdentificationTypeId = loanInsurancePolicySaveDto.BeneficiaryIdentificationTypeId,
                    BeneficiaryName = loanInsurancePolicySaveDto.BeneficiaryName,
                    DisabilityMoreInfo = loanInsurancePolicySaveDto.DisabilityMoreInfo,
                    Diseases = loanInsurancePolicySaveDto.Diseases,
                    DiseasesMoreInfo = loanInsurancePolicySaveDto.DiseasesMoreInfo,
                    PlannedMedicalProcidureMoreInfo = loanInsurancePolicySaveDto.PlannedMedicalProcidureMoreInfo
                };

                await _loanInsurancePolicyRepository.Create(newLoanInsurencePolicy);
            }
            else
            {
                loanInsurencePolicy.Address = loanInsurancePolicySaveDto.Address;
                loanInsurencePolicy.IsPublicPerson = loanInsurancePolicySaveDto.IsPublicPerson;
                loanInsurencePolicy.LiabilitiesValue = loanInsurancePolicySaveDto.LiabilitiesValue;
                loanInsurencePolicy.HousingValue = loanInsurancePolicySaveDto.HousingValue;
                loanInsurencePolicy.HousingTypeId = loanInsurancePolicySaveDto.HousingTypeId;
                loanInsurencePolicy.HIVActive = loanInsurancePolicySaveDto.HIVActive;
                loanInsurencePolicy.Height = loanInsurancePolicySaveDto.Height;
                loanInsurencePolicy.AssetValue = loanInsurancePolicySaveDto.AssetValue;
                loanInsurencePolicy.Beneficiaries = loanInsurancePolicySaveDto.Beneficiaries;
                loanInsurencePolicy.BirthCityId = loanInsurancePolicySaveDto.BirthCityId;
                loanInsurencePolicy.BuiltArea = loanInsurancePolicySaveDto.BuiltArea;
                loanInsurencePolicy.BuiltgDate = loanInsurancePolicySaveDto.BuiltgDate;
                loanInsurencePolicy.CityId = loanInsurancePolicySaveDto.CityId;
                loanInsurencePolicy.CivilStatusId = loanInsurancePolicySaveDto.CivilStatusId;
                loanInsurencePolicy.Convicted = loanInsurancePolicySaveDto.Convicted;
                loanInsurencePolicy.CountryNationalityId = loanInsurancePolicySaveDto.CountryNationalityId;
                loanInsurencePolicy.Disability = loanInsurancePolicySaveDto.Disability;
                loanInsurencePolicy.LifeEnsure = loanInsurancePolicySaveDto.LifeEnsure;
                loanInsurencePolicy.MonthlyExpenses = loanInsurancePolicySaveDto.MonthlyExpenses;
                loanInsurencePolicy.MonthlyIncome = loanInsurancePolicySaveDto.MonthlyIncome;
                loanInsurencePolicy.NumberChildren = loanInsurancePolicySaveDto.NumberChildren;
                loanInsurencePolicy.Occupation = loanInsurancePolicySaveDto.Occupation;
                loanInsurencePolicy.OccupationActivityId = loanInsurancePolicySaveDto.OccupationActivityId;
                loanInsurencePolicy.PlannedMedicalProcidure = loanInsurancePolicySaveDto.PlannedMedicalProcidure;
                loanInsurencePolicy.ResidenceCityId = loanInsurancePolicySaveDto.ResidenceCityId;
                loanInsurencePolicy.Weight = loanInsurancePolicySaveDto.Weight;
                loanInsurencePolicy.ApartmentNumber = loanInsurancePolicySaveDto.ApartmentNumber;
                loanInsurencePolicy.BasementNumber = loanInsurancePolicySaveDto.BasementNumber;
                loanInsurencePolicy.IncomeOrigin = loanInsurancePolicySaveDto.IncomeOrigin;
                loanInsurencePolicy.BuildingFloorsNumber = loanInsurancePolicySaveDto.BuildingFloorsNumber;
                loanInsurencePolicy.BeneficiaryIdentificationNumber = loanInsurancePolicySaveDto.BeneficiaryIdentificationNumber;
                loanInsurencePolicy.BeneficiaryIdentificationTypeId = loanInsurancePolicySaveDto.BeneficiaryIdentificationTypeId;
                loanInsurencePolicy.BeneficiaryName = loanInsurancePolicySaveDto.BeneficiaryName;
                loanInsurencePolicy.DisabilityMoreInfo = loanInsurancePolicySaveDto.DisabilityMoreInfo;
                loanInsurencePolicy.Diseases = loanInsurancePolicySaveDto.Diseases;
                loanInsurencePolicy.DiseasesMoreInfo = loanInsurancePolicySaveDto.DiseasesMoreInfo;
                loanInsurencePolicy.PlannedMedicalProcidureMoreInfo = loanInsurancePolicySaveDto.PlannedMedicalProcidureMoreInfo;


                await _loanInsurancePolicyRepository.Update(loanInsurencePolicy);
            }

            result.Result = await _loanInsurancePolicyRepository.GetAllLoanInsurancePolicyByLoanApplicationId(loanInsurancePolicySaveDto.LoanApplicationId);
            return result;
        }
    }
}
