using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Clientify
{
    public interface IChangeStatusDealService
    {
        Task Execute(string dealStatus, int loanApplicationId);
    }
}
