using Bangk.Common.Dto;
using Bangk.Domain.Contract.Clientify;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Clientify
{
    public class ChangeStateDealService : IChangeStateDealService
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IConfiguration _configuration;
        public ChangeStateDealService(ILoanApplicationRepository loanApplicationRepository, IConfiguration configuration)
        {
            _loanApplicationRepository = loanApplicationRepository;
            _configuration = configuration;
        }

        public async Task Execute(int stateId, int loanApplicationId)
        {
            var apiClient = new ClientifyService();
            // Autenticación y obtención del token
            await apiClient.LoginAsync("v1/api-auth/obtain_token/", _configuration.GetSection("ClientifyUser").Value,
                _configuration.GetSection("ClientifyPassword").Value);

            var loanApplication = await _loanApplicationRepository.GetById(loanApplicationId);
            var updateStateClientifyDto = new UpdateStateClientifyDto
            {
                status = stateId
            };

            await apiClient.PatchAsync($"/v1/deals/{loanApplication.ClientifyId}/", updateStateClientifyDto);
        }
    }
}
