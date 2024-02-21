using System.ComponentModel;

namespace Bangk.Common.Enums
{
    public enum ClientStateEnum
    {
        Potencial = 109,
        Prospecto = 110,
        Activo = 111,
        Inactivo = 116,
        PerdidoRechazo = 138
    }

    public enum ApplicationStateHistoryEnum
    {
        SolicitudCreada = 119,
        RevisionDeExtracto = 120,
        CotizacionEnviada = 121,
        CotizaciónAceptada = 123,
        FormulariosDiligenciados = 125,
        EstudioSarlaft = 126,
        EstudioListaRestrictivaSBS = 127,
        PolizaEmitida = 128,
        PolizaPagada = 129,
        CartaDeEndosoRealizada = 130,
        EndosoAceptada = 131,
        ProximoARenovar = 132,
        Renovado = 133,
        Vencido = 134,
        RechazadoPorLaAseguradora = 135,
        CotizaciónRechazada = 136,
        CotizaciónNoRespondida = 137

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
