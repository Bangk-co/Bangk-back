namespace Bangk.Common.Dto
{
    public class ResetPasswordDto
    {
       public string Password { get; set; }
       public string PasswordConfirm { get; set; }  
       public string Hash { get; set; }  
    }
}
