using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.City
{
    public interface IGetCityService
    {
        Task<List<CityDto>> Execute(int stateId);
    }
}
