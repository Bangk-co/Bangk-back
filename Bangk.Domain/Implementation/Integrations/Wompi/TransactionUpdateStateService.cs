using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Domain.Contract.Integrations.Wompi;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Integrations.Wompi
{
    public class TransactionUpdateStateService : ITransactionUpdateStateService
    {
        private readonly IPetEnsuranceApplicationRepository _petEnsuranceApplicationRepository;
        private readonly IApplicationTransactionRepository _applicationTransactionRepository;
        private readonly IMedEnsuranceApplicationRepository _medEnsuranceApplicationRepository;

        public TransactionUpdateStateService(IPetEnsuranceApplicationRepository petEnsuranceApplicationRepository,
            IApplicationTransactionRepository applicationTransactionRepository,
            IMedEnsuranceApplicationRepository medEnsuranceApplicationRepository)
        {
            _petEnsuranceApplicationRepository = petEnsuranceApplicationRepository;
            _applicationTransactionRepository = applicationTransactionRepository;
            _medEnsuranceApplicationRepository = medEnsuranceApplicationRepository;
        }

        public async Task Execute(UpdateTransactionWebHookDto transactionUpdateDto)
        {
            if (string.Equals(transactionUpdateDto.Data.Transaction.Status, "APPROVED"))
            {
                await ApprovedTransactionProcess(transactionUpdateDto);
            }
            else
            {
                await RejectedTransactionProcess(transactionUpdateDto);
            }
        }

        private async Task ApprovedTransactionProcess(UpdateTransactionWebHookDto transactionUpdateDto)
        {
            switch (transactionUpdateDto.Data.Transaction.Reference)
            {
                case string reference when reference.Contains("PET"):
                    var petApplication = await _petEnsuranceApplicationRepository.GetPetApplicationByReference(transactionUpdateDto.Data.Transaction.Reference);
                    petApplication.Status = (int)InsuranceStatesEnum.Created;
                    await _petEnsuranceApplicationRepository.Update(petApplication);
                    break;
                case string reference when reference.Contains("MED"):
                    var medApplication = await _medEnsuranceApplicationRepository.GetMedEnsuranceApplicationByReference(transactionUpdateDto.Data.Transaction.Reference);
                    medApplication.Status = (int)InsuranceStatesEnum.Created;
                    await _medEnsuranceApplicationRepository.Update(medApplication);
                    break;
            }

            //Create log transaction
            var transactionAdd = new ApplicationTransaction
            {
                Amount = transactionUpdateDto.Data.Transaction.Amount_In_Cents,
                Currency = transactionUpdateDto.Data.Transaction.Currency,
                Reference = transactionUpdateDto.Data.Transaction.Reference,
                Status = transactionUpdateDto.Data.Transaction.Status,
                TransactionCreationDate = transactionUpdateDto.Sent_At,
                TransactionNumber = transactionUpdateDto.Data.Transaction.Id,
                PaymentMethodType = transactionUpdateDto.Data.Transaction.Payment_Method_Type,
            };

            await _applicationTransactionRepository.Create(transactionAdd);

        }


        private async Task RejectedTransactionProcess(UpdateTransactionWebHookDto transactionUpdateDto)
        {
            switch (transactionUpdateDto.Data.Transaction.Reference)
            {
                case string reference when reference.Contains("PET"):
                    var petApplication = await _petEnsuranceApplicationRepository.GetPetApplicationByReference(transactionUpdateDto.Data.Transaction.Reference);
                    petApplication.Status = (int)InsuranceStatesEnum.Rejected;
                    await _petEnsuranceApplicationRepository.Update(petApplication);
                    break;
                case string reference when reference.Contains("MED"):
                    var medApplication = await _medEnsuranceApplicationRepository.GetMedEnsuranceApplicationByReference(transactionUpdateDto.Data.Transaction.Reference);
                    medApplication.Status = (int)InsuranceStatesEnum.Rejected;
                    await _medEnsuranceApplicationRepository.Update(medApplication);
                    break;
            }

            //Create log transaction
            var transactionAdd = new ApplicationTransaction
            {
                Amount = transactionUpdateDto.Data.Transaction.Amount_In_Cents,
                Currency = transactionUpdateDto.Data.Transaction.Currency,
                Reference = transactionUpdateDto.Data.Transaction.Reference,
                Status = transactionUpdateDto.Data.Transaction.Status,
                TransactionCreationDate = transactionUpdateDto.Sent_At,
                TransactionNumber = transactionUpdateDto.Data.Transaction.Id,
                PaymentMethodType = transactionUpdateDto.Data.Transaction.Payment_Method_Type,
            };

            await _applicationTransactionRepository.Create(transactionAdd);

        }

    }
}
