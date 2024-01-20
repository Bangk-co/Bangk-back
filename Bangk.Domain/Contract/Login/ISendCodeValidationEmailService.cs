using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Login
{
    public interface ISendCodeValidationEmailService
    {
        Task<ResponseDto<string>> Execute(SendCodeValidationEmailDto sendCodeValidationEmailDto);
    }
}
