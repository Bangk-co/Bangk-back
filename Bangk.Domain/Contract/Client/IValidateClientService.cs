using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Client
{
    public interface IValidateClientService
    {
        Task<ResponseDto<bool>> Execute(string documentNumber, string email, int documentType);
    }
}
