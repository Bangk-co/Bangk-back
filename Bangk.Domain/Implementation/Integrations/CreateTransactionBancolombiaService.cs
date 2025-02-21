using Bangk.Domain.Contract.Integrations;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.Integrations
{
    public class CreateTransactionBancolombiaService : ICreateTransactionBancolombiaService
    {
        private readonly IConfiguration _configuration;
        public CreateTransactionBancolombiaService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
