using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Clientify
{
    public interface IChangeStateDealService
    {
        Task Execute(int stateId, int loanApplicationId);
    }
}
