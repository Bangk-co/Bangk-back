using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public interface ILoanApplicationDocumentRepository : IGenericRepository<LoanApplicationDocument, LoanApplicationDocumentDto>
    {
        Task<ClientDocumentDto> GetClientDocumentByClientId(int clientId);

        Task<DownloadDocumentDto> GetBlobUrlForDocument(int documentId);
    }
}
