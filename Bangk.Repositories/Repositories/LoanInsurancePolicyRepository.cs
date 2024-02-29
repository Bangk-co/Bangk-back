using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class LoanInsurancePolicyRepository : GenericRepository<LoanInsurancePolicy, LoanInsurancePolicyDto>, ILoanInsurancePolicyRepository
    {
        public LoanInsurancePolicyRepository(AppDbContext context) : base(context)
        {

        }

        public async Task<LoanInsurancePolicyDto> GetAllLoanInsurancePolicyByLoanApplicationId(int loanApplicationId)
        {
            var query = from l in Context.LoanApplication
                        join c in Context.Clients on l.ClientId equals c.Id
                        join lp in Context.LoanInsurancePolicy on l.Id equals lp.LoanApplicationId into t
                        from sublp in t.DefaultIfEmpty()
                        join bc in Context.Cities on sublp.BirthCityId equals bc.Id into ct
                        from subbc in ct.DefaultIfEmpty()
                        join rc in Context.Cities on sublp.ResidenceCityId equals rc.Id into cr
                        from subrc in cr.DefaultIfEmpty()
                        join ic in Context.Cities on sublp.CityId equals ic.Id into ci
                        from subic in ci.DefaultIfEmpty()
                        where l.Id == loanApplicationId
                        select new LoanInsurancePolicyDto
                        {
                            LoanApplicationId = l.Id,
                            Address = sublp.Address,
                            AssetValue = sublp.AssetValue,
                            Beneficiaries = sublp.Beneficiaries,
                            BirthCityId = sublp.BirthCityId,
                            BuiltArea = sublp.BuiltArea,
                            BuiltgDate = sublp.BuiltgDate,
                            CityId = sublp.CityId,
                            CivilStatusId = sublp.CivilStatusId,
                            Convicted = sublp.Convicted,
                            CountryCodePhoneId = c.CountryCodePhoneId,
                            CountryNationalityId = sublp.CountryNationalityId,
                            Disability = sublp.Disability,
                            DocumentIdentificationTypeId = c.DocumenIdentificationTypeId,
                            DocumentIssueDate = c.DocumentIssueDate,
                            Email = c.Email,
                            Height = sublp.Height,
                            HIVActive = sublp.HIVActive,
                            HousingTypeId = sublp.HousingTypeId,
                            HousingValue = sublp.HousingValue,
                            IdentificationNumber = c.IdentificationNumber,
                            IsPublicPerson = sublp.IsPublicPerson,
                            LastName = c.LastName,
                            LiabilitiesValue = sublp.LiabilitiesValue,
                            LifeEnsure = sublp.LifeEnsure,
                            MonthlyExpenses = sublp.MonthlyExpenses,
                            MonthlyIncome = sublp.MonthlyIncome,
                            Name = c.Name,
                            NumberChildren = sublp.NumberChildren,
                            Occupation = sublp.Occupation,
                            OccupationActivityId = sublp.OccupationActivityId,
                            PhoneNumber = c.PhoneNumber,
                            PlannedMedicalProcidure = sublp.PlannedMedicalProcidure,
                            ResidenceCityId = sublp.ResidenceCityId,
                            Weight = sublp.Weight,
                            ApartmentNumber = sublp.ApartmentNumber,
                            BasementNumber = sublp.BasementNumber,
                            IncomeOrigin = sublp.IncomeOrigin,
                            BeneficiaryIdentificationNumber = sublp.BeneficiaryIdentificationNumber,
                            BeneficiaryIdentificationTypeId = sublp.BeneficiaryIdentificationTypeId,
                            BeneficiaryName = sublp.BeneficiaryName,
                            DisabilityMoreInfo = sublp.DisabilityMoreInfo,
                            Diseases = sublp.Diseases,
                            DiseasesMoreInfo = sublp.DiseasesMoreInfo,
                            PlannedMedicalProcidureMoreInfo = sublp.PlannedMedicalProcidureMoreInfo,
                            ClienId = c.Id,
                            BirthDate = c.BirthDate,
                            GenderId = c.GenderId,
                            BuildingFloorsNumber = sublp.BuildingFloorsNumber,
                            LoanBalance = l.LoanBalance,
                            MonthlyCostInsurance = l.MonthlyCostInsurance,
                            BirthStateId = subbc.StateId,
                            BirthCountryId = subbc.State.CountryId,
                            ResidenceCountryId = subrc.State.CountryId,
                            ResidenceStateId = subrc.StateId,
                            StateId = subic.StateId 
                        };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<LoanInsurancePolicy> GetLoanInsurancePolicyByLoadApplicationId(int loanApplicationId)
        {
            var query = Context.LoanInsurancePolicy.Where(l => l.LoanApplicationId  == loanApplicationId);
            return await query.FirstOrDefaultAsync();
        }
    }
}
