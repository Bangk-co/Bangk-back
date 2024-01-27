using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Enums
{
    public enum ApplicationStateEnum
    {
        Potencial = 109
    }

    public enum DocumentType
    {
        [Description("Extractos bancarios")]

        BankStatement,
        [Description("Póliza # ")]
        InsurancePolicy


    }
}
