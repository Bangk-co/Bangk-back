using Bangk.Domain.Contract.Integrations.Wompi;
using Coravel.Invocable;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.BackgroundTask
{
    public class PaymentAssistanceTryTwoJob : IInvocable
    {
        private readonly IPaymentAssistanceTryService _paymentAssistanceTryOneService;
        public PaymentAssistanceTryTwoJob(IPaymentAssistanceTryService paymentAssistanceTryOneService)
        {
            _paymentAssistanceTryOneService = paymentAssistanceTryOneService;
        }

        public async Task Invoke()
        {
            await Execute();
        }

        public async Task Execute()
        {
            await _paymentAssistanceTryOneService.Execute(2);
        }
    }
}
