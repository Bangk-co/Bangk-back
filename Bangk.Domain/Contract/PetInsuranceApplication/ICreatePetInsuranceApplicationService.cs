using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.PetInsuranceApplication
{
    public interface ICreatePetInsuranceApplicationService
    {
        Task<ResponseDto<int>> Execute(PetInsuranceDto petInsuranceDto);
    }
}
