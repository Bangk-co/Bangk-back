using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
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
                        join icv in Context.Cities on sublp.CityVehiculeId equals icv.Id into icvt
                        from subicvt in icvt.DefaultIfEmpty()
                        where l.Id == loanApplicationId
                        select new LoanInsurancePolicyDto
                        {
                            LoanApplicationId = l.Id,
                            ResidenceAddress = sublp.ResidenceAddress,
                            AssetValue = sublp.AssetValue,
                            BirthCityId = sublp.BirthCityId,
                            BuiltArea = sublp.BuiltArea,
                            BuiltgDate = sublp.BuiltgDate,
                            Beneficiaries = sublp.Beneficiaries,
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
                            HasBondPublicPerson = sublp.HasBondPublicPerson,
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
                            StateId = subic.StateId,
                            LoanTypeId = l.LoanTypeId,
                            CountryVehiculeId = subicvt.State.CountryId,
                            StateVehiculeId = subicvt.StateId,
                            CityVehiculeId = subicvt.Id,
                            VehicleLicensePlate = sublp.VehicleLicensePlate,
                            Beneficiary = (
                            from b in Context.Beneficiaries
                            where b.LoanInsurancePolicyId == sublp.Id
                            select new BeneficiariesDto

                            {
                                Id = b.Id,
                                Name = b.Name,
                                LastName = b.LastName,
                                LoanInsurancePolicyId = b.LoanInsurancePolicyId,
                                IdentificationNumber = b.IdentificationNumber,
                                IdentificationTypeId = b.IdentificationTypeId,
                                RelationshipId = b.RelationshipId,
                                PercentInherit = b.PercentInherit
                            }).ToList()



                        };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<LoanInsurancePolicy> GetLoanInsurancePolicyByLoadApplicationId(int loanApplicationId)
        {
            var query = Context.LoanInsurancePolicy.Where(l => l.LoanApplicationId == loanApplicationId);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<ClientSBSInfoDto> GetClientSBSInfoDtoyByLoanApplicationId(int loanApplicationId)
        {
            var query = from lp in Context.LoanInsurancePolicy
                        join bc in Context.Cities on lp.BirthCityId equals bc.Id into ct
                        from subbc in ct.DefaultIfEmpty()

                        join rc in Context.Countries on lp.CountryNationalityId equals rc.Id into cr
                        from subrc in cr.DefaultIfEmpty()
                        where lp.LoanApplicationId == loanApplicationId
                        select new ClientSBSInfoDto
                        {
                            BirthCountryId = subbc.State.Countries.Id,
                            CountryNationalityId = subrc.Id,
                            BirthCountrySBSId = subbc.State.Countries.SBSId,
                            CountryNationalitySBSId = subrc.SBSId
                        };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<ClientSarlaftInfoDto> GetClientSarlaftInfoDtoyByLoanApplicationId(int loanApplicationId)
        {
            var query = from l in Context.LoanApplication
                        join lp in Context.LoanInsurancePolicy on l.Id equals lp.LoanApplicationId
                        join lq in Context.LoanQuote on l.Id equals lq.LoanApplicationId
                        where lp.LoanApplicationId == loanApplicationId
                        select new ClientSarlaftInfoDto
                        {
                           HasBondPublicPerson = lp.HasBondPublicPerson.HasValue ? lp.HasBondPublicPerson.Value : false,
                           IsPublicPerson = lp.IsPublicPerson.HasValue ? lp.IsPublicPerson.Value : false,
                           LoanBalance = l.LoanBalance,
                           YearlyFee = ((long)lq.YearlyFee)
                        };

            return await query.FirstOrDefaultAsync();
        }

        public int GetLastConsecutive()
        {
            var query = Context.LoanInsurancePolicy.OrderByDescending(p => p.Consecutive).FirstOrDefault();
            return query?.Consecutive ?? 0;
        }
    }
}
