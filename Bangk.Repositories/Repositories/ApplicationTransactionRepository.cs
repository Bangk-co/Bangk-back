using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class ApplicationTransactionRepository : GenericRepository<ApplicationTransaction, ApplicationTransactionDto>, IApplicationTransactionRepository
    {
        public ApplicationTransactionRepository(AppDbContext context) : base(context)
        {

        }
    }
}
