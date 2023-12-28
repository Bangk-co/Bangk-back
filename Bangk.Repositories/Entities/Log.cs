using System;

namespace Bangk.Repositories.Entities
{
    public class Log : EntityBase
    {
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Exception { get; set; }
    }
}
