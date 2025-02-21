using Bangk.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class UserPaymentRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public DocumentType DocumentIdentificationType { get; set; }
        public string DocumentIdentificationNumber { get; set; }
        public string TransactionToken { get; set; }
        public int IdTransaction { get; set; }
        public int Amount { get; set; }
    }
}
