using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface IClientRepository : IGenericRepository<Client, ClientDto>
    {
        Task<bool> ExistClientByEmailAndIdentificationNumber(string email, string identificationNumber);
        IQueryable<ClientDto> GetClientListQuery(string searchValue, string sortField, string sortOrder, ClaimsPrincipal user);

        IQueryable<ClientInfoDto> GetClientByIdInfo(int clientId);
        Task<Client> GetClientByLoanApplicationId(int loanApplicationId);
        IQueryable<ClientReportDto> GetReportClientsSarlaft(ApplicationStateHistoryEnum stateloanApplication);
        IQueryable<int> GetLoanApplicationIdsWithState(ApplicationStateHistoryEnum stateloanApplication);
        IQueryable<ClientReport2Dto> GetReportClientsSarlaftLogs(ApplicationStateHistoryEnum stateloanApplication);
        string GetDiseasesNames(string diseaseIds);
        IQueryable<ClientPolicyDto> GetPolicyClient(int clientId);
        //IQueryable<Beneficiaries2JobDto> GetBeneficiariesLogs(int loanInsurancePolicyId);
        List<Beneficiaries2JobDto> GetBeneficiariesByPolicyIdLogs(int policyId);
        //IQueryable<BeneficiariesJobDto> GetBeneficiaries(int loanInsurancePolicyId);
        List<BeneficiariesJobDto> GetBeneficiariesByPolicyId(int policyId);



    }
}
