using Bangk.Common.Dto;
using Bangk.Domain.Contract.Clientify;
using Bangk.Repositories.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Dynamic;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Clientify
{
    public class ChangeStatusDealService : IChangeStatusDealService
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IConfiguration _configuration;
        public ChangeStatusDealService(ILoanApplicationRepository loanApplicationRepository, IConfiguration configuration)
        {
            _loanApplicationRepository = loanApplicationRepository;
            _configuration = configuration;
        }

        public async Task Execute(string dealStatus, int loanApplicationId)
        {
            var apiClient = new ClientifyService();
            // Autenticación y obtención del token
            await apiClient.LoginAsync("v1/api-auth/obtain_token/", _configuration.GetSection("ClientifyUser").Value,
                _configuration.GetSection("ClientifyPassword").Value);
            
            var loanApplication = await _loanApplicationRepository.GetById(loanApplicationId);
            var stageUrl = string.Equals(dealStatus, "Solicitud") ? "https://api.clientify.net/v1/deals/pipelines/stages/193422/"
                : "https://api.clientify.net/v1/deals/pipelines/stages/193423/";
            var update = new UpdateStatusClientifyDto
            {
                pipeline_stage_desc = dealStatus,
                pipeline_stage = stageUrl
            };

            await apiClient.PatchAsync($"/v1/deals/{loanApplication.ClientifyId}/", update);
        }
    }
}
