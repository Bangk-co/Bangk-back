namespace Bangk.Repositories.Entities
{
    public class ValidationSBSLog : EntityBase
    {
        public bool RegistroValidado { get; set; }
        public string DescripcionError { get; set; }
        public int TipoRestriccionOfac { get; set; }
        public int OrigenRestriccionOfac { get; set; }
        public string DetalleListaOfac { get; set; }
        public string DetalleIdsErrores { get; set; }
        public string TipoRestriccionOfacDesc { get; set; }
        public string OrigenRestriccionOfacDesc { get; set; }
    }
}
