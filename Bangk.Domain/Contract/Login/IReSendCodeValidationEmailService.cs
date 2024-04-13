using Bangk.Common.Dto;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Login
{
    public interface IReSendCodeValidationEmailService
    {
        Task<ResponseDto<string>> Execute(string email);
    }
}
