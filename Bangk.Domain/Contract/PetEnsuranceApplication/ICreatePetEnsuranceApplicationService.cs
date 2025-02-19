using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.PetEnsuranceApplication
{
    public interface ICreatePetEnsuranceApplicationService
    {
        Task<ResponseDto<int>> Execute(PetEnsuranceDto petEnsuranceDto);
    }
}
