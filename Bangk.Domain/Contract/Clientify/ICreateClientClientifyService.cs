using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Clientify
{
    public interface ICreateClientClientifyService
    {
        Task<long> Execute(CreateClientDto createClientDto);

    }
}
