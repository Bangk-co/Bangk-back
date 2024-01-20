using Bangk.Common.Dto;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;

namespace Bangk.Repositories.Repositories
{
    public class LoanApplicationDocumentRepository : GenericRepository<LoanApplicationDocument, LoanApplicationDocumentDto>, ILoanApplicationDocumentRepository
    {
        public LoanApplicationDocumentRepository(AppDbContext context) : base(context)
        {
        }
    }
}
