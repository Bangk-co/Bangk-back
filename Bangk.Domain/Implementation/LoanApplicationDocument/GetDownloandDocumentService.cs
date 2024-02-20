using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplicationDocument;
using Bangk.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanApplicationDocument
{
    public class GetDownloandDocumentService: IGetDownloadDocumentService
    {
        private readonly ILoanApplicationDocumentRepository _loanApplicationDocuementRepository;
        public GetDownloandDocumentService(ILoanApplicationDocumentRepository loanApplicationDocumentRepository)
        {
            _loanApplicationDocuementRepository = loanApplicationDocumentRepository;
        }

        public async Task<DownloadDocumentDto> Execute(int documentId)
        {
            try
            {
                var DownloadDocumentDto = await _loanApplicationDocuementRepository.GetBlobUrlForDocument(documentId);

                
                return DownloadDocumentDto;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error al obtener la URL del blob del documento: {ex.Message}");
                throw;
            }
        }
    }
}
