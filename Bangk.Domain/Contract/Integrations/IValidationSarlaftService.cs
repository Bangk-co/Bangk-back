using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Integrations
{
    public interface IValidationSarlaftService
    {
        Task Execute(int loanApplicationId);
    }
}
