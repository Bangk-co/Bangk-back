using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class UpdatePasswordDto
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
