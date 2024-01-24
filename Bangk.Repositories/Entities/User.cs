using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bangk.Repositories.Entities
{
    public class User : EntityBase
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public Role Role { get; set; }
        public string PasswordResetGuid { get; set; }

        public virtual ICollection<LoanApplication> LoanApplications { get; set; }
    }
}
