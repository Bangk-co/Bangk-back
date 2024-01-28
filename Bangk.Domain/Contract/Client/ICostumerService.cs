using Bangk.Common.Dto;
using Bangk.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract.Client
{
    public interface ICostumerService
    {
        Task<PaginationDto<ClientDto>> Execute(int currentPage, int pageSize, string searchValue, string sortField, string sortOrder);
        

    }
}
