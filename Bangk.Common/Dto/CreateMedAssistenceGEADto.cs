using System;
using System.Collections.Generic;

namespace Bangk.Common.Dto
{
    public class CreateMedAssistenceGEADto
    {
        public int ApplicationId { get; set; }
        public string DocumentNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone {  get; set; }
        public int DocumentType { get; set; }
        public int PaymentOcurrencyId { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<CreateMedGEADto> MedPersons { get; set; }
    }

    public class CreateMedGEADto
    {
        public string DocumentNumber { get; set; }
        public int DocumentType { get; set; }
    }
}
