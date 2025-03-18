using System.ComponentModel.DataAnnotations.Schema;

namespace Bangk.Repositories.Entities
{
    public class Pet : EntityBase
    {
        [ForeignKey("PetEnsuranceApplication")] 
        public int PetEnsuranceApplicationId { get; set; }
        public int PetTypeId { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
