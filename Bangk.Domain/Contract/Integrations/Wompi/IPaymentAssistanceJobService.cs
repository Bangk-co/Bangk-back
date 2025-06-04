using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Integrations.Wompi
{
    public interface IPaymentAssistanceJobService
    {
        Task Execute();
    }
}
