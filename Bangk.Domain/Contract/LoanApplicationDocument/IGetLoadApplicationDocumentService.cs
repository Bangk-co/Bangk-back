using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.LoanApplicationDocument
{
    public interface IGetLoadApplicationDocumentService
    {
        Task<ClientDocumentDto> Execute(int ClientId);
    }
}
