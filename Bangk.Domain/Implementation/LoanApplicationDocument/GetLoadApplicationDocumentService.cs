using Bangk.Common.Dto;
using Bangk.Domain.Contract.LoanApplicationDocument;
using Bangk.Repositories.Repositories;
using System.Threading.Tasks;

namespace Bangk.Domain.Implementation.LoanApplicationDocument
{
    public class GetLoadApplicationDocumentService : IGetLoadApplicationDocumentService
    {
        private readonly ILoanApplicationDocumentRepository _loanApplicationDocuementRepository;
        public GetLoadApplicationDocumentService(ILoanApplicationDocumentRepository loanApplicationDocumentRepository)
        {
            _loanApplicationDocuementRepository = loanApplicationDocumentRepository;
        }

        public async Task<ClientDocumentDto> Execute(int ClientId)
        {
            return await _loanApplicationDocuementRepository.GetClientDocumentByClientId(ClientId);
        }
    }
}
