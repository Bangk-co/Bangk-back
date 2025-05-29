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
        private readonly IUserRepository _userRepository;

        public CreateMedPersonalInsuredService(IMedPersonInsuredRepository medPersonalInsuredRepository,
            IMedEnsuranceApplicationRepository medEnsuranceApplicationRepository, 
            IClientRepository clientRepository, 
            IUserRepository userRepository)
        {
            _medPersonalInsuredRepository = medPersonalInsuredRepository;
            _medEnsuranceApplicationRepository = medEnsuranceApplicationRepository;
            _clientRepository = clientRepository;
            _userRepository = userRepository;
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
            var clientExist = await _clientRepository.GetClientByIdentificationNumberDocumentTypeEmail(medPersonInsuredsDto.DocumentTypeId, medPersonInsuredsDto.DocumentNumber, medPersonInsuredsDto.Email);

            if (clientExist == null)
            {
                var clientExistByDocument = await _clientRepository.GetClientByIdentificationNumberAndType(medPersonInsuredsDto.DocumentTypeId, medPersonInsuredsDto.DocumentNumber);
                if (clientExistByDocument == null)
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
                    clientExistByDocument.Email = medPersonInsuredsDto.Email;
                    clientExistByDocument.PermissionTreatPersonalDataDate = medPersonInsuredsDto.TreatPErsonalDatadate;
                    clientExistByDocument.DocumenIdentificationTypeId = medPersonInsuredsDto.DocumentTypeId;
                    clientExistByDocument.IdentificationNumber = medPersonInsuredsDto.DocumentNumber;
                    clientExistByDocument.LastName = medPersonInsuredsDto.LastName;
                    clientExistByDocument.Name = medPersonInsuredsDto.Name;
                    clientExistByDocument.CountryCodePhoneId = medPersonInsuredsDto.CountryCodePhoneId;
                    clientExistByDocument.PhoneNumber = medPersonInsuredsDto.PhoneNumber;
                    await _clientRepository.Update(clientExistByDocument);
                    clientId = clientExistByDocument.Id;
                }
            }
            else
            {
                clientId = clientExist.Id;
            }

            var userReview = await _userRepository.GetUserWithLessApplication();
            var medEnsuranceApplication = new Repositories.Entities.MedInsuranceApplication()
            {
                ClientId = clientId,
                PaymentOcurrencyId = medPersonInsuredsDto.PaymentOcurrencyId,
                Status = (int)InsuranceStatesEnum.PendingToApprove,
                ReviewerId = userReview.Id,
            };

            await _medEnsuranceApplicationRepository.Create(medEnsuranceApplication);

            var nextPaymentDate = medPersonInsuredsDto.PaymentOcurrencyId switch
            {
                (int)MethodPaymentTimeEnum.Mensual => medEnsuranceApplication.CreationDate.AddMonths(1),
                (int)MethodPaymentTimeEnum.Semestral => medEnsuranceApplication.CreationDate.AddMonths(6),
                _ => medEnsuranceApplication.CreationDate.AddYears(1)
            };

            medEnsuranceApplication.NextPaymentDate = nextPaymentDate;
            await _medEnsuranceApplicationRepository.Update(medEnsuranceApplication);

            var beneficiaries = new List<Repositories.Entities.MedPersonInsureds>();
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
                    TreatPErsonalDatadate = item.TreatPErsonalDatadate,
                    CountryCodePhoneId = item.CountryCodePhoneId,
                    LastName = item.LastName,
                };

                beneficiaries.Add(beneficiary);
            }

            if (medPersonInsuredsDto.IsBeneficiary)
            {
                var beneficiary = new Repositories.Entities.MedPersonInsureds
                {
                    DocumentTypeId = medPersonInsuredsDto.DocumentTypeId,
                    DocumentNumber = medPersonInsuredsDto.DocumentNumber,
                    Name = medPersonInsuredsDto.Name,
                    Email = medPersonInsuredsDto.Email,
                    PhoneNumber = medPersonInsuredsDto.PhoneNumber,
                    MedEnsuraceApplicationId = medEnsuranceApplication.Id,
                    TreatPErsonalDatadate = medPersonInsuredsDto.TreatPErsonalDatadate,
                    CountryCodePhoneId = medPersonInsuredsDto.CountryCodePhoneId,
                    LastName = medPersonInsuredsDto.LastName,
                };

                beneficiaries.Add(beneficiary);
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
