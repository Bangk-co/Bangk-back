using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Login
{
    public interface ISendCodeOtpEmailService
    {
        Task<ResponseDto<string>> Execute(SendCodeValidationEmailDto sendCodeValidationEmailDto);
    }
}
