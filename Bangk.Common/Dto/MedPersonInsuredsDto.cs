using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class MedPersonInsuredsDto
    {
        public int DocumentTypeId { get; set; }
        public string DocumentNumber { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public bool Deleted { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int MedEnsuraceApplicationId { get; set; }
        public DateTime TreatPErsonalDatadate { get; set; }
        public CreateMedEnsuranceApplicationDto MedEnsuranceApplicationDto { get; set; }
    }

    public class CreateMedEnsuranceApplicationDto
    {
        public int MethodPaymentId { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public int Status { get; set; }
    }
}
