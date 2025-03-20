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
        private readonly IClientRepository _clientRepository;
        public CreateMedPersonalInsuredService(IMedPersonInsuredRepository medPersonalInsuredRepository, IMedEnsuranceApplicationRepository medEnsuranceApplicationRepository, IClientRepository clientRepository)
        {
            _medPersonalInsuredRepository = medPersonalInsuredRepository;
            _medEnsuranceApplicationRepository = medEnsuranceApplicationRepository;
            _clientRepository = clientRepository;
        }

        public async Task<ResponseDto<int>> Execute(MedPersonInsuredsDto medPersonInsuredsDto)
        {
            var result = new ResponseDto<int>
            {
                Result = 0
            };

            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

            if (medPersonInsuredsDto.BeneficiaryInsurance == null || !medPersonInsuredsDto.BeneficiaryInsurance.Any())
            {
                result.HasErrors = true;
                result.Errors.Add("Debe proporcionar al menos un beneficiario para el seguro.");
                return result;
            }

            int clientId = 0;
            var clientExist = await _clientRepository.ExistClientByEmailAndIdentificationNumber(
                medPersonInsuredsDto.Email,
                medPersonInsuredsDto.DocumentNumber
            );

            if (!clientExist)
            {
                var client = new Repositories.Entities.Client
                {
                    Email = medPersonInsuredsDto.Email,
                    PermissionTreatPersonalDataDate = medPersonInsuredsDto.TreatPErsonalDatadate,
                    DocumenIdentificationTypeId = medPersonInsuredsDto.DocumentTypeId,
                    IdentificationNumber = medPersonInsuredsDto.DocumentNumber,
                    LastName = medPersonInsuredsDto.LastName,
                    Name = medPersonInsuredsDto.Name,
                    CountryCodePhoneId = medPersonInsuredsDto.CountryCodePhoneId,
                    PhoneNumber = medPersonInsuredsDto.PhoneNumber,
                };

                await _clientRepository.Create(client);
                clientId = client.Id;
            }
            else
            {
                var client = await _clientRepository.GetClientByEmailAndIdentificationNumber(
                    medPersonInsuredsDto.Email,
                    medPersonInsuredsDto.DocumentNumber
                );
                clientId = client.Id;
            }

            var medEnsuranceApplication = new Repositories.Entities.MedEnsuranceApplication()
            {
                ClientId = clientId,
                MethodPaymentId = medPersonInsuredsDto.MethodPaymentId,
                Status = (int)InsuranceStatesEnum.PendingToCreate,
            };

            await _medEnsuranceApplicationRepository.Create(medEnsuranceApplication);

            var beneficiaries = new List<Repositories.Entities.MedPersonInsureds>();

            int order = 1;
            foreach (var item in medPersonInsuredsDto.BeneficiaryInsurance)
            {
                var beneficiary = new Repositories.Entities.MedPersonInsureds
                {
                    DocumentTypeId = item.DocumentTypeId,
                    DocumentNumber = item.DocumentNumber,
                    Name = item.Name,
                    Email = item.Email,
                    PhoneNumber = item.PhoneNumber,
                    MedEnsuraceApplicationId = medEnsuranceApplication.Id,
                };

                beneficiaries.Add(beneficiary);
                order++;
            }

            if (beneficiaries.Count > 0)
            {
                await _medPersonalInsuredRepository.CreateBulk(beneficiaries);
            }

            result.Result = medEnsuranceApplication.Id;
            scope.Complete();
            return result;
        }


    }
}
