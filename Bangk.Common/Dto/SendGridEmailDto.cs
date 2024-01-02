namespace Bangk.Common.Dto
{
    public class SendGridEmailDto
    {
        public string Key { get; set; }
        public string From { get; set; }
        public string Name { get; set; }
        public string email { get; set; }
        public object Data { get; set; }
        public string TemplateId { get; set; }

    }
}
