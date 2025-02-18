using Bangk.Common.Dto;
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
    public class CreateMedPersonalInsuredService: ICreateMedPersonalInsuredService
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
                Status = medPersonInsuredsDto.MedEnsuranceApplicationDto.Status
            };

            await _medEnsuranceApplicationRepository.Create(MedEnsuranceApplication);

            var MedPersonalInsured = new Repositories.Entities.MedPersonInsureds()
            {
                DocumentTypeId = medPersonInsuredsDto.DocumentTypeId,
                DocumentNumber = medPersonInsuredsDto.DocumentNumber,
                Name = medPersonInsuredsDto.Name,
                Deleted = medPersonInsuredsDto.Deleted,
                Email = medPersonInsuredsDto.Email,
                PhoneNumber = medPersonInsuredsDto.PhoneNumber,
                MedEnsuraceApplicationId = MedEnsuranceApplication.Id,
                TreatPErsonalDatadate = medPersonInsuredsDto.TreatPErsonalDatadate
            };

            await _medPersonalInsuredRepository.Create(MedPersonalInsured);

            result.Result = MedPersonalInsured.Id;
            scope.Complete();
            return result;
        }
    }
}
