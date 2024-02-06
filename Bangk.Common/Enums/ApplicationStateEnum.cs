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
        Potencial = 109,
        Prospecto = 110,
        Activo = 111
    }

    public enum ApplicationStateHistoryEnum
    {
        SolicitudCreada = 119,
        RevisionDeExtracto = 120,
        CotizacionEnviada = 121,
        CotizaciónAceptada = 122
    }

    public enum DocumentType
    {
        [Description("Extractos bancarios")]

        BankStatement,
        [Description("Póliza # ")]
        InsurancePolicy


    }
}
