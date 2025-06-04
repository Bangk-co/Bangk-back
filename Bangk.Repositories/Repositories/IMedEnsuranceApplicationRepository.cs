using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface IMedEnsuranceApplicationRepository: IGenericRepository<MedInsuranceApplication, MedEnsuranceApplicationDto>
    {
        Task<MedInsuranceApplication> GetMedEnsuranceApplicationByReference(string reference);
        Task<CreateMedAssistenceGEADto> GetMedAssistenceGEA(string reference);
        Task<List<PaymentJobDto>> GetMedAssistencePendingToPay(DateTime dateTime);
        Task<List<PaymentJobDto>> GetMedAssistencePayment(string applicationReference);
    }
}
