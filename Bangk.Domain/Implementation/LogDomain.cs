using Bangk.Domain.Contract;
using Bangk.Repositories.Entities;
using Bangk.Repositories.Repositories;

namespace Bangk.Domain.Implementation
{
    public class LogDomain : ILogDomain
    {
        public LogDomain(ILogRepository logRepository)
        {
        }
    }
}