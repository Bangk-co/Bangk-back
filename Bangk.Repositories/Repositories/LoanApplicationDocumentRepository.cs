using Bangk.Common.Dto;
using Bangk.Common.Enums;
using Bangk.Repositories.Context;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Infrastructure;
using EnumsNET;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
                                  where c.Id == clientId
                                  select new
                                  {
                                      ClientId = c.Id,
                                      CreateIdentificationDate = c.CreationDate,
                                      IdentificationUrl = c.IdentificationFileUrl,
                                      LoanApplications = c.LoanApplications.Select(s => new
                                      {
                                          LoanApplicationId = s.Id,
                                          LoanApplicationDocuments = s.LoanApplicationDocuments.Select(d => new
                                          {
                                              d.DocumentType,
                                              d.CreationDate,
                                              d.Url
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
                        CreateDocumentDate = doc.CreationDate,
                        DocumentUrl = doc.Url,
                        DocumentName = $"{((DocumentType)doc.DocumentType).AsString(EnumFormat.Description) } Solicitud {s.LoanApplicationId}"
                    }).ToList()
                }));

            var clientDto = new ClientDocumentDto
            {
                ClientId = clientInfo.ClientId,
                CreateIdentificationDate = clientInfo.CreateIdentificationDate,
                IdentificationUrl = clientInfo.IdentificationUrl,
                ClientDocumentListDtos = clientDocumentDtos.ToList()
            };

            return clientDto;
        }
    }
}