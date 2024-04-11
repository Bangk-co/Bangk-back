using System.Threading.Tasks;

namespace Bangk.Domain.Contract.ScheduleIntegration
{
    public interface IScheduleIntegrationService
    {
        Task CreateNextTrySBS(int loanApplicationId, int typeValidation);
    }
}
