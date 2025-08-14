using System;
using System.Collections.Generic;

namespace Bangk.Common.Dto
{
    public class CreatePetAssistenceGEADto
    {
        public int ApplicationId { get; set; }
        public string DocumentNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone {  get; set; }
        public int DocumentType { get; set; }
        public int PaymentOcurrencyId { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<CreatePetGEADto> Pets { get; set; }
        public long? Amount { get; set; }
    }

    public class CreatePetGEADto
    {
        public int PetType { get; set; }
        public int Age { get; set; }
        public string Breed { get; set; }
        public string Color { get; set; }
        public string Name { get; set; }
        public int Order {  get; set; }
    }
}
