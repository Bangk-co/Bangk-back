using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Integrations
{
    public interface IValidationSBSService
    {
        Task Execute(int loanApplicationId, bool isCallFromApi = false);
    }
}
