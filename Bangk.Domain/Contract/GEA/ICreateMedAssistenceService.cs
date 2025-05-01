using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.GEA
{
    public interface ICreateMedAssistenceService
    {
        Task<ResponseDto<bool>> Execute(string applicationReference);
    }
}
