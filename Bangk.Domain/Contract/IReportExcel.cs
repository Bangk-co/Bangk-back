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
        Task GenerateExcel(string remoteDirectory, string fileName, IQueryable<ClientReportDto> clients);
        Task CreateSarlaftReportLogs();
        Task GenerateExcelLogs(string remoteDirectory, string fileName, IQueryable<ClientReport2Dto> clients);
        Task SaveLastConsecutive(int consecutive, int loanPolicyId);



    }
}
