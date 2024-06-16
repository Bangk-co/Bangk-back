using Bangk.Common.Enums;
using Bangk.Domain.Contract.LoanQuote;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanQuote
{
    public class ValidateLoanApplicationQouteService : IValidateLoanApplicationQouteService
    {
        private readonly ILoanApplicationStateHistoryRepository _loanApplicationStateHistoryRepository;
        private readonly IConfiguration _configuration;
        private readonly IClientRepository _clientRepository;
        public ValidateLoanApplicationQouteService(ILoanApplicationStateHistoryRepository loanApplicationStateHistoryRepository,
            IConfiguration configuration,
            IClientRepository clientRepository)
        {
            _loanApplicationStateHistoryRepository = loanApplicationStateHistoryRepository;
            _configuration = configuration;
            _clientRepository = clientRepository;
        }

        public async Task<string> Execute(int loanApplicationId)
        {
            var loanApplicationHistory = await _loanApplicationStateHistoryRepository.GetLastStateByLoanApplicationId(loanApplicationId);
            var client = await _clientRepository.GetClientByLoanApplicationId(loanApplicationId);
            if (loanApplicationHistory.StateId == (int)ApplicationStateHistoryEnum.CotizacionEnviada)
            {
                return _configuration.GetSection("LoanQuote:UrlFront").Value + $"?requestId={loanApplicationId}&email={client.Email}";
            }

            if(loanApplicationHistory.StateId == (int)ApplicationStateHistoryEnum.CotizaciónAceptada)
            {
                return _configuration.GetSection("LoanQuoteAccept:UrlFront").Value;
            }

            return _configuration.GetSection("LoanQuoteReject:UrlFront").Value;
        }
    }
}
