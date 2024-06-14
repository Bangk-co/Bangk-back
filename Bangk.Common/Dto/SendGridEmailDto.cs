using System.Collections.Generic;

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
        public List<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();

    }

    public class AttachmentDto
    {
        public string Filename { get; set; }
        public string Content { get; set; }
        public string Type { get; set; }
    }
}
