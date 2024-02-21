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
                        join lp in Context.LoanInsurancePolicy on l.Id equals lp.LoanApplicationId
                        where l.Id == loanApplicationId
                        select new LoanInsurancePolicyDto
                        {
                            LoanApplicationId = l.Id,
                            Address = lp.Address,
                            AssetValue = lp.AssetValue,
                            Beneficiaries = lp.Beneficiaries,
                            BirthCityId = lp.BirthCityId,
                            BuiltArea = lp.BuiltArea,
                            BuiltgDate = lp.BuiltgDate,
                            CityId = lp.CityId,
                            CivilStatusId = lp.CivilStatusId,
                            Convicted = lp.Convicted,
                            CountryCodePhoneId = c.CountryCodePhoneId,
                            CountryNationalityId = lp.CountryNationalityId,
                            Disability = lp.Disability,
                            DocumentIdentificationTypeId = c.DocumenIdentificationTypeId,
                            DocumentIssueDate = c.DocumentIssueDate,
                            Email = c.Email,
                            Height = lp.Height,
                            HIVActive = lp.HIVActive,
                            HousingTypeId = lp.HousingTypeId,
                            HousingValue = lp.HousingValue,
                            IdentificationNumber = c.IdentificationNumber,
                            IsPublicPerson = lp.IsPublicPerson,
                            LastName = c.LastName,
                            LiabilitiesValue = lp.LiabilitiesValue,
                            LifeEnsure = lp.LifeEnsure,
                            MonthlyExpenses = lp.MonthlyExpenses,
                            MonthlyIncome = lp.MonthlyIncome,
                            Name = c.Name,
                            NumberChildren = lp.NumberChildren,
                            Occupation = lp.Occupation,
                            OccupationActivityId = lp.OccupationActivityId,
                            PhoneNumber = c.PhoneNumber,
                            PlannedMedicalProcidure = lp.PlannedMedicalProcidure,
                            ResidenceCityId = lp.ResidenceCityId,
                            Weight = lp.Weight

                        };

            return await query.FirstOrDefaultAsync();
        }
    }
}
