using Bangk.Domain.Contract.Integrations.Wompi;
using Coravel.Invocable;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.BackgroundTask
{
    public class PaymentAssistanceJob : IInvocable
    {
        private readonly IPaymentAssistanceJobService _paymentAssistanceJobService;
        public PaymentAssistanceJob(IPaymentAssistanceJobService paymentAssistanceJobService)
        {
            _paymentAssistanceJobService = paymentAssistanceJobService;
        }

        public async Task Invoke()
        {
            await Execute();
        }

        public async Task Execute()
        {
            await _paymentAssistanceJobService.Execute();
        }
    }
}
