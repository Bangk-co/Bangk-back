using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Linq;

namespace Bangk.Repositories.Repositories
{
    public class ApplicationTransactionRepository : GenericRepository<ApplicationTransaction, ApplicationTransactionDto>, IApplicationTransactionRepository
    {
        public ApplicationTransactionRepository(AppDbContext context) : base(context)
        {

        }

        public ApplicationTransaction GetApplicationTransactionByTransactionNumber(string transactionNumber)
        {
            var query = from ap in Context.ApplicationTransaction
                         where ap.TransactionNumber == transactionNumber
                         select ap;
            return query.FirstOrDefault();
        }
    }
}
