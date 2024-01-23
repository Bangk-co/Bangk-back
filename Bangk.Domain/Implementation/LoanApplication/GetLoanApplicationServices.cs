using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplication;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanApplication
{
    public class GetLoanApplicationServices : ILoanApplicationServices
    {

        private readonly ILoanApplicationRepository _ILoanApplicationRepository;

        public GetLoanApplicationServices(ILoanApplicationRepository LoanApplicationRepository)
        {
            _ILoanApplicationRepository = LoanApplicationRepository;
        }

        public async Task<List<LoanApplicationDto>> Execute(int ClientId)
        {
            try
            {
                var loanApplications = _ILoanApplicationRepository.GetLoanApplicationInfo(ClientId);
                return loanApplications;
            }catch (Exception ex)
            {
                throw new Exception($"Se ha generado un error consultando la información de los créditos del cliente -> {ex.Message}");
            }
        }
    }
}
