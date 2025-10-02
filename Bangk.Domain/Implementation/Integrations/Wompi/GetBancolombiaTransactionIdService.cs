using Bangk.Common.Dto;
using Bangk.Domain.Contract.Integrations.Wompi;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Integrations.Wompi
{
    public class GetBancolombiaTransactionIdService : IGetBancolombiaTransactionIdService
    {
        private readonly IMedEnsuranceApplicationRepository _medInsuranceApplicationRepository;
        private readonly IPetInsuranceApplicationRepository _petInsuranceApplicationRepository;
        public GetBancolombiaTransactionIdService(IMedEnsuranceApplicationRepository medInsuranceApplicationRepository,
           IPetInsuranceApplicationRepository petInsuranceApplicationRepository)
        {
            _medInsuranceApplicationRepository = medInsuranceApplicationRepository;
            _petInsuranceApplicationRepository = petInsuranceApplicationRepository;
        }

        public async Task<string> Execute(int applicationId, bool isMed)
        {
            if (isMed)
            {
                var application = await _medInsuranceApplicationRepository.GetById(applicationId);
                return application.BancolombiaTransactionId;
            }

            var applicationPet = await _petInsuranceApplicationRepository.GetById(applicationId);
            return applicationPet.BancolombiaTransactionId;
        }
    }
}