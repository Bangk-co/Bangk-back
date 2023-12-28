using System.Collections.Generic;

namespace Bangk.Common.Dto
{
    public class ResponseDto<T>
    {
        public bool HasErrors { get; set; }
        public List<string> Errors { get; set; } = new();
        public T Result { get; set; }
    }
}
