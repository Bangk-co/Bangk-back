using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Integrations.Wompi
{
    public interface IPaymentAssistanceTryService
    {
        Task Execute(int countFailed);
    }
}
