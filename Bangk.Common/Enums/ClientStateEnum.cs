using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Enums
{
    public enum ClientStateEnum
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
        CotizaciónAceptada = 123,
        FormulariosDiligenciados = 124,
        EstudioSarlaft = 125,
        EstudioListaRestrictivaSBS = 126,
        PolizaEmitida = 127,
        PolizaPagada = 128,
        CartaDeEndosoRealizada = 129,
        EndosoAceptada = 130,
        ProximoARenovar = 131,
        Renovado = 132,
        Vencido = 133,
        RechazadoPorLaAseguradora = 134,
        CotizaciónRechazada = 135,
        CotizaciónNoRespondida = 136


    }

    public enum DocumentType
    {
        [Description("Extractos bancarios")]

        BankStatement,
        [Description("Póliza # ")]
        InsurancePolicy,
        [Description("Carta de endoso")]
        EndorsementLetters
    }

    public enum SMLVEnum
    {
        SMLV = 124
    }
}
