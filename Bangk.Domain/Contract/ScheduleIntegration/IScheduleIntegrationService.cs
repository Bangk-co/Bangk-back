using System.Threading.Tasks;

namespace Bangk.Domain.Contract.ScheduleIntegration
{
    public interface IScheduleIntegrationService
    {
        Task CreateNextTrySBSOrSarlaft(int loanApplicationId, int typeValidation);
    }
}
