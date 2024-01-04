namespace Bangk.Common.Dto
{
    public class UserDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public int? CompanyId { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string PasswordResetGuid { get; set; }
        public bool? Deleted { get; set; }
    }
}
