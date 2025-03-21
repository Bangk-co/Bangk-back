using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bangk.Repositories.Entities
{
    public class MedPersonInsureds : EntityBase
    {
        [ForeignKey("Setting")]
        public int DocumentTypeId { get; set; }
        public string DocumentNumber { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public bool Deleted { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [ForeignKey("MedEncuranceApplication")]
        public int MedEnsuraceApplicationId { get; set; }
        public DateTime TreatPErsonalDatadate { get; set; }
        [ForeignKey("Setting")]
        public int CountryCodePhoneId { get; set; }
    }
}
