using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.MedPersonalInsureds;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Bangk.Domain.Implementation.MedPersonalInsureds
{
    public class CreateMedPersonalInsuredService : ICreateMedPersonalInsuredService
    {
        private readonly IMedPersonInsuredRepository _medPersonalInsuredRepository;
        private readonly IMedEnsuranceApplicationRepository _medEnsuranceApplicationRepository;
        public CreateMedPersonalInsuredService(IMedPersonInsuredRepository medPersonalInsuredRepository, IMedEnsuranceApplicationRepository medEnsuranceApplicationRepository)
        {
            _medPersonalInsuredRepository = medPersonalInsuredRepository;
            _medEnsuranceApplicationRepository = medEnsuranceApplicationRepository;

        }

        public async Task<ResponseDto<int>> Execute(MedPersonInsuredsDto medPersonInsuredsDto)
        {
            var result = new ResponseDto<int>
            {
                Result = 0
            };

            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

            var MedEnsuranceApplication = new Repositories.Entities.MedEnsuranceApplication()
            {
                MethodPaymentId = medPersonInsuredsDto.MedEnsuranceApplicationDto.MethodPaymentId,
                Deleted = medPersonInsuredsDto.MedEnsuranceApplicationDto.Deleted,
                Status = (int)InsuranceStatesEnum.PendingToCreate
            };

            await _medEnsuranceApplicationRepository.Create(MedEnsuranceApplication);

            var medPersonalInsureds = new List<Repositories.Entities.MedPersonInsureds>();
            if (medPersonInsuredsDto != null && medPersonInsuredsDto.MedEnsuranceApplicationDto != null)
            {
                var insureds = new List<MedPersonInsuredsDto> { medPersonInsuredsDto };
                foreach (var item in insureds)
                {
                    var medPersonalInsured = new Repositories.Entities.MedPersonInsureds
                    {
                        DocumentTypeId = item.DocumentTypeId,
                        DocumentNumber = item.DocumentNumber,
                        Name = item.Name,
                        Deleted = item.Deleted,
                        Email = item.Email,
                        PhoneNumber = item.PhoneNumber,
                        MedEnsuraceApplicationId = MedEnsuranceApplication.Id,
                        TreatPErsonalDatadate = item.TreatPErsonalDatadate
                    };

                    medPersonalInsureds.Add(medPersonalInsured);
                }
            }

            if (medPersonalInsureds.Count > 0)
            {
                await _medPersonalInsuredRepository.CreateBulk(medPersonalInsureds);
            }

            result.Result = MedEnsuranceApplication.Id;
            scope.Complete();
            return result;
        }
    }
}
