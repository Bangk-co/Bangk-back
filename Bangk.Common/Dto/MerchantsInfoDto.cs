using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto.MecharntsInfoDto
{
    public class MecharntsInfoDto
    {
        public Data Data { get; set; }
        public Meta Meta { get; set; }
    }

    public class Data
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Contact_Name { get; set; }
        public string Phone_Number { get; set; }
        public bool Active { get; set; }
        public string Logo_Url { get; set; }
        public string Legal_Name { get; set; }
        public string Legal_Id_Type { get; set; }
        public string Legal_Id { get; set; }
        public string Public_Key { get; set; }
        public List<string> Accepted_Currencies { get; set; }
        public string Fraud_Javascript_Key { get; set; }
        public List<object> Fraud_Groups { get; set; } // Puedes cambiar `object` por una clase específica si hay estructura
        public List<string> Accepted_Payment_Methods { get; set; }
        public List<PaymentMethod> Payment_Methods { get; set; }
        public PresignedAcceptance Presigned_Acceptance { get; set; }
        public PresignedPersonalDataAuth Presigned_Personal_Data_Auth { get; set; }
    }

    public class PaymentMethod
    {
        public string Name { get; set; }
        public List<PaymentProcessor> Payment_Processors { get; set; }
    }

    public class PaymentProcessor
    {
        public string Name { get; set; }
    }

    public class PresignedAcceptance
    {
        public string Acceptance_Token { get; set; }
        public string Permalink { get; set; }
        public string Type { get; set; }
    }

    public class PresignedPersonalDataAuth
    {
        public string Acceptance_Token { get; set; }
        public string Permalink { get; set; }
        public string Type { get; set; }
    }

    public class Meta
    {
        // Puedes agregar propiedades aquí si el objeto "meta" tiene contenido en el futuro
    }
}
