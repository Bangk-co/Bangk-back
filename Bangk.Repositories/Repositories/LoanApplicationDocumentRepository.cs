using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using EnumsNET;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace Bangk.Repositories.Repositories
{
    public class LoanApplicationDocumentRepository : GenericRepository<LoanApplicationDocument, LoanApplicationDocumentDto>, ILoanApplicationDocumentRepository
    {
        public LoanApplicationDocumentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<ClientDocumentDto> GetClientDocumentByClientId(int clientId)
        {
            var clientInfoQuery = from c in Context.Clients
                                  join l in Context.LoanApplication on c.Id equals l.ClientId
                                  join ld in Context.LoanApplicationDocument on l.Id equals ld.LoanApplicationId
                                  where c.Id == clientId && ld.DocumentType == (int)DocumentType.Identification
                                  select new
                                  {
                                      ClientId = c.Id,
                                      CreateIdentificationDate = c.CreationDate,
                                      DocumentIdentificationId = ld.Id,
                                      LoanApplications = c.LoanApplications.Select(s => new
                                      {
                                          LoanApplicationId = s.Id,
                                          LoanApplicationDocuments = s.LoanApplicationDocuments.Where(d => d.DocumentType != (int)DocumentType.Identification).Select(d => new
                                          {
                                              d.Id,
                                              d.DocumentType,
                                              d.CreationDate
                                          })
                                      })
                                  };

            var clientInfo = await clientInfoQuery.FirstOrDefaultAsync();
            var clientDocumentDtos = clientInfo.LoanApplications.SelectMany(s => s.LoanApplicationDocuments
                .GroupBy(d => d.DocumentType)
                .Select(g => new ClientDocumentListDto
                {
                    DocumentType = g.Key,
                    DocumentByTypeDtos = g.Select(doc => new DocumentByTypeDto
                    {
                        DocumentId = doc.Id,
                        CreateDocumentDate = doc.CreationDate,
                        DocumentName = $"{((DocumentType)doc.DocumentType).AsString(EnumFormat.Description) } Solicitud {s.LoanApplicationId}"
                    }).ToList()
                }));

            var clientDto = new ClientDocumentDto
            {
                ClientId = clientInfo.ClientId,
                CreateIdentificationDate = clientInfo.CreateIdentificationDate,
                DocumentIdentificationId = clientInfo.DocumentIdentificationId,
                ClientDocumentListDtos = clientDocumentDtos.ToList()
            };

            return clientDto;
        }

        public async Task<DownloadDocumentDto> GetBlobUrlForDocument(int documentId)
        {
            return await Context.LoanApplicationDocument
                .Where(d => d.Id == documentId)
                .Select(d => new DownloadDocumentDto { DocumentId = d.Id, Url = d.Url })
                .FirstOrDefaultAsync();
        }
    }
}