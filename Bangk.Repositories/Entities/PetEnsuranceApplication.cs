using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bangk.Repositories.Entities
{
    public class PetEnsuranceApplication : EntityBase
    {
        [ForeignKey("Setting")]
        public int MethodPayment {  get; set; }
        public int Status { get; set; }
        [ForeignKey("Setting")]
        public int DocumentTypeId { get; set; }
        public string DocumentNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime TreatPersonalDataDate { get; set; }
    }
}
