using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface IPetInsuranceApplicationRepository : IGenericRepository<PetInsuranceApplication, PetInsuranceDto>
    {
        Task<PetInsuranceApplication> GetPetApplicationByReference(string reference);
        Task<CreatePetAssistenceGEADto> GetPetAssistenceGEA(string reference);
        Task<List<PaymentJobDto>> GetPetAssistancePendingToPay(DateTime dateTime);
        Task<List<PaymentJobDto>> GetPetAssistancePayment(string applicationReference);
        Task<PaymentJobDto> GetPetAssistanceByReference(string applicationReference);
        Task<PaymentJobDto> GetPetAssistanceByApplicationId(int applicationId);
    }
}
