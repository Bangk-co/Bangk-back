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
        Task<byte[]> CreateSarlaftReport();
        Task<byte[]> GenerateExcel(string remoteDirectory, string fileName, IQueryable<ClientReportDto> clients);
        Task<byte[]> CreateSarlaftReportLogs();
        Task<byte[]> GenerateExcelLogs(string remoteDirectory, string fileName, IQueryable<ClientReport2Dto> clients);
        Task SaveLastConsecutive(int consecutive, int loanPolicyId);
        Task<byte[]> CreateExcel(IQueryable<ClientReportDto> clients);
        Task<byte[]> CreateExcelLogs(IQueryable<ClientReport2Dto> clients);



    }
}
