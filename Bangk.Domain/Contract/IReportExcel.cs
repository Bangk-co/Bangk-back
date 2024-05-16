using Bangk.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Domain.Contract
{
    public interface IReportExcel
    {
        Task CreateSarlaftReport();
        Task GenerateExcel(string dateFile, IQueryable<ClientReportDto> clients);
        Task CreateSarlaftReportLogs();
        Task GenerateExcelLogs(string dateFile, IQueryable<ClientReport2Dto> clients);



    }
}
