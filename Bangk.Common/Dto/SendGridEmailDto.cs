namespace Bangk.Common.Dto
{
    public class SendGridEmailDto
    {
        public string Key { get; set; }
        public string From { get; set; }
        public string Name { get; set; }
        public string email { get; set; }
        public string subject { get; set; }
        public string htmlMessage { get; set; }
    }
}
