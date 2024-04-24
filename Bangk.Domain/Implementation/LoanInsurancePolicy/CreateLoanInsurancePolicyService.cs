using Bangk.Business.Extension.SenderGrid;
using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.LoanInsurancePolicy;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Transactions;
using static System.Formats.Asn1.AsnWriter;

namespace Bangk.Domain.Implementation.Client
{
    public class CreateLoanInsurancePolicyService : ICreateLoanInsurancePolicyService
    {
        private readonly ILoanInsurancePolicyRepository _loanInsurancePolicyRepository;
        private readonly IBeneficiaryRepository _beneficiaryRepository;
        private readonly INotificationRepository _notificationRepository;
 
        public CreateLoanInsurancePolicyService(ILoanInsurancePolicyRepository loanInsurancePolicyRepository, IBeneficiaryRepository beneficiaryRepository,
            INotificationRepository notificationRepository)
        {
            _loanInsurancePolicyRepository = loanInsurancePolicyRepository;
            _beneficiaryRepository = beneficiaryRepository;
            _notificationRepository = notificationRepository;
            

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
                await CreateLoanInsurancePolicy(loanInsurancePolicySaveDto);
            }
            else
            {
                await UpdateLoanInsurancePolicy(loanInsurancePolicySaveDto, loanInsurencePolicy);
            }
           
            var notifications = await _notificationRepository.GetNotificationByApplicationId(loanInsurancePolicySaveDto.LoanApplicationId);
            if(notifications != null && notifications.Count > 0)
            {
               await _notificationRepository.DeleteBulk(notifications);
            }

            await CreateNotification(loanInsurancePolicySaveDto.LoanApplicationId);
            result.Result = await _loanInsurancePolicyRepository.GetAllLoanInsurancePolicyByLoanApplicationId(loanInsurancePolicySaveDto.LoanApplicationId);
            return result;
        }

        private async Task CreateLoanInsurancePolicy(LoanInsurancePolicySaveDto loanInsurancePolicySaveDto)
        {
            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);
            var newLoanInsurencePolicy = new Repositories.Entities.LoanInsurancePolicy
            {
                Address = loanInsurancePolicySaveDto.Address,
                IsPublicPerson = loanInsurancePolicySaveDto.IsPublicPerson,
                AcceptanceOriginOfFunds = loanInsurancePolicySaveDto.AcceptanceOriginOfFunds,
                
                LiabilitiesValue = loanInsurancePolicySaveDto.LiabilitiesValue,
                HasBondPublicPerson = loanInsurancePolicySaveDto.HasBondPublicPerson,
                HousingValue = loanInsurancePolicySaveDto.HousingValue,
                HousingTypeId = loanInsurancePolicySaveDto.HousingTypeId,
                HIVActive = loanInsurancePolicySaveDto.HIVActive,
                Height = loanInsurancePolicySaveDto.Height,
                AssetValue = loanInsurancePolicySaveDto.AssetValue,
                BirthCityId = loanInsurancePolicySaveDto.BirthCityId,
                BuiltArea = loanInsurancePolicySaveDto.BuiltArea,
                BuiltgDate = loanInsurancePolicySaveDto.BuiltgDate,
                Beneficiaries = loanInsurancePolicySaveDto.Beneficiaries,
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
                DisabilityMoreInfo = loanInsurancePolicySaveDto.DisabilityMoreInfo,
                Diseases = loanInsurancePolicySaveDto.Diseases,
                DiseasesMoreInfo = loanInsurancePolicySaveDto.DiseasesMoreInfo,
                PlannedMedicalProcidureMoreInfo = loanInsurancePolicySaveDto.PlannedMedicalProcidureMoreInfo,
                CityVehiculeId = loanInsurancePolicySaveDto.CityVehiculeId,
                VehicleLicensePlate = loanInsurancePolicySaveDto.VehicleLicensePlate
                
            };

            await _loanInsurancePolicyRepository.Create(newLoanInsurencePolicy);
            

            if (loanInsurancePolicySaveDto.beneficiary != null && loanInsurancePolicySaveDto.beneficiary.Count > 0
                && loanInsurancePolicySaveDto.beneficiary.Count <= 7 && newLoanInsurencePolicy.Beneficiaries.HasValue && newLoanInsurencePolicy.Beneficiaries.Value)
            {
                foreach (var b in loanInsurancePolicySaveDto.beneficiary)
                {
                    var beneficiary = new Repositories.Entities.Beneficiary
                    {
                        LoanInsurancePolicyId = newLoanInsurencePolicy.Id,
                        Name = b.Name,
                        LastName = b.LastName,
                        IdentificationTypeId = b.IdentificationTypeId,
                        IdentificationNumber = b.IdentificationNumber,
                        RelationshipId = b.RelationshipId,
                        PercentInherit = b.PercentInherit
                    };

                    await _beneficiaryRepository.Create(beneficiary);
                }
            }
            scope.Complete();
        }
        private async Task UpdateLoanInsurancePolicy(LoanInsurancePolicySaveDto loanInsurancePolicySaveDto, LoanInsurancePolicy loanInsurencePolicy)
        {
            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);
            loanInsurencePolicy.Address = loanInsurancePolicySaveDto.Address;
            loanInsurencePolicy.IsPublicPerson = loanInsurancePolicySaveDto.IsPublicPerson;
            loanInsurencePolicy.LiabilitiesValue = loanInsurancePolicySaveDto.LiabilitiesValue;
            loanInsurencePolicy.HasBondPublicPerson = loanInsurancePolicySaveDto.HasBondPublicPerson;
            loanInsurencePolicy.HousingValue = loanInsurancePolicySaveDto.HousingValue;
            loanInsurencePolicy.HousingTypeId = loanInsurancePolicySaveDto.HousingTypeId;
            loanInsurencePolicy.HIVActive = loanInsurancePolicySaveDto.HIVActive;
            loanInsurencePolicy.Height = loanInsurancePolicySaveDto.Height;
            loanInsurencePolicy.AssetValue = loanInsurancePolicySaveDto.AssetValue;
            loanInsurencePolicy.BirthCityId = loanInsurancePolicySaveDto.BirthCityId;
            loanInsurencePolicy.BuiltArea = loanInsurancePolicySaveDto.BuiltArea;
            loanInsurencePolicy.BuiltgDate = loanInsurancePolicySaveDto.BuiltgDate;
            loanInsurencePolicy.Beneficiaries = loanInsurancePolicySaveDto.Beneficiaries;
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
            loanInsurencePolicy.DisabilityMoreInfo = loanInsurancePolicySaveDto.DisabilityMoreInfo;
            loanInsurencePolicy.Diseases = loanInsurancePolicySaveDto.Diseases;
            loanInsurencePolicy.DiseasesMoreInfo = loanInsurancePolicySaveDto.DiseasesMoreInfo;
            loanInsurencePolicy.PlannedMedicalProcidureMoreInfo = loanInsurancePolicySaveDto.PlannedMedicalProcidureMoreInfo;
            loanInsurencePolicy.CreationDate = DateTime.UtcNow;
            loanInsurencePolicy.CityVehiculeId = loanInsurancePolicySaveDto.CityVehiculeId;
            loanInsurencePolicy.VehicleLicensePlate = loanInsurancePolicySaveDto.VehicleLicensePlate;

            await _loanInsurancePolicyRepository.Update(loanInsurencePolicy);
            

            if (loanInsurancePolicySaveDto.beneficiary != null && loanInsurancePolicySaveDto.beneficiary.Count > 0 && loanInsurancePolicySaveDto.beneficiary.Count <= 7
                && loanInsurancePolicySaveDto.Beneficiaries.HasValue && loanInsurancePolicySaveDto.Beneficiaries.Value)
            {
                foreach (var b in loanInsurancePolicySaveDto.beneficiary)
                {
                    if (b.Id != 0)
                    {
                        var beneficiaryInBD = await _beneficiaryRepository.GetById(b.Id);
                        if (beneficiaryInBD != null)
                        {
                            beneficiaryInBD.LoanInsurancePolicyId = loanInsurencePolicy.Id;
                            beneficiaryInBD.Name = b.Name;
                            beneficiaryInBD.IdentificationTypeId = b.IdentificationTypeId;
                            beneficiaryInBD.IdentificationNumber = b.IdentificationNumber;
                            beneficiaryInBD.RelationshipId = b.RelationshipId;
                            beneficiaryInBD.PercentInherit = b.PercentInherit;
                            beneficiaryInBD.CreationDate = DateTime.UtcNow;
                            await _beneficiaryRepository.Update(beneficiaryInBD);
                        }
                    }
                    else
                    {
                        var beneficiary = new Repositories.Entities.Beneficiary
                        {
                            LoanInsurancePolicyId = loanInsurencePolicy.Id,
                            Name = b.Name,
                            LastName = b.LastName,
                            IdentificationTypeId = b.IdentificationTypeId,
                            IdentificationNumber = b.IdentificationNumber,
                            RelationshipId = b.RelationshipId,
                            PercentInherit = b.PercentInherit,
                            CreationDate = DateTime.UtcNow
                        };

                        await _beneficiaryRepository.Create(beneficiary);
                    }
                }
            }

            
            scope.Complete();
        }

        private async Task CreateNotification(int loanApplicationId)
        {
            var notification1 = new Notification { ApplicationId = loanApplicationId, NotificationType = 1, DateToSendNotification = DateTime.UtcNow.AddHours(23).AddMinutes(53) };
            await _notificationRepository.Create(notification1);

            var notification2 = new Notification { ApplicationId = loanApplicationId, NotificationType = 1, DateToSendNotification = DateTime.UtcNow.AddDays(3) };
            await _notificationRepository.Create(notification2);

            var notification3 = new Notification { ApplicationId = loanApplicationId, NotificationType = 1, DateToSendNotification = DateTime.UtcNow.AddDays(6) };
            await _notificationRepository.Create(notification3);

            var notification4 = new Notification { ApplicationId = loanApplicationId, NotificationType = 1, DateToSendNotification = DateTime.UtcNow.AddDays(20) };
            await _notificationRepository.Create(notification4);

            var notification5 = new Notification { ApplicationId = loanApplicationId, NotificationType = 2, DateToSendNotification = DateTime.UtcNow.AddDays(30) };
            await _notificationRepository.Create(notification5);
        }
    }
}
