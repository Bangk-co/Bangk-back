using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.MedPersonalInsureds
{
    public interface ICreateMedPersonalInsuredService
    {

        Task<ResponseDto<int>> Execute(MedPersonInsuredsDto medPersonInsuredsDto);

    }
}