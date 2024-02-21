using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Country
{
    public interface IGetCountryService
    {
        Task<List<CountryDto>> Execute();
    }
}
