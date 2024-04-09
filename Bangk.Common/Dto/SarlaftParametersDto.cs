namespace Bangk.Common.Dto
{
    public class SarlaftParametersDto
    {
        public string TipoConsulta { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string DigitoVerificacion { get; set; }
        public string FechaExpedicion { get; set; }
        public string NumeroCelular { get; set; }
        public bool EsPPE { get; set; }
        public bool VinculosPPE { get; set; }
        public long ValorAsegurado { get; set; }
        public long PrimaKMs { get; set; }
        public string TipoParticipante { get; set; }
        public string VinculoLaboral { get; set; }
        public string Intermediario { get; set; }
        public string Sponsor { get; set; }
        public string Ramo { get; set; }
    }
}
