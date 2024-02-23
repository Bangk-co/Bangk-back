using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.State
{
    public interface IGetStatesService
    {
        Task<List<StateDto>> Execute(int countryId);
    }
}
