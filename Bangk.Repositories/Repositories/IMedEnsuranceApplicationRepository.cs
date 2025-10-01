using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface IMedEnsuranceApplicationRepository: IGenericRepository<MedInsuranceApplication, MedEnsuranceApplicationDto>
    {
        Task<MedInsuranceApplication> GetMedInsuranceApplicationByReference(string reference);
        Task<CreateMedAssistenceGEADto> GetMedAssistanceGEA(string reference);
        Task<List<PaymentJobDto>> GetMedAssistancePendingToPay(DateTime dateTime);
        Task<List<PaymentJobDto>> GetMedAssistancePayment(string applicationReference);
        Task<PaymentJobDto> GetMedAssistanceByReference(string applicationReference);
        Task<PaymentJobDto> GetMedAssistanceByApplicationId(int applicationId);
    }
}
