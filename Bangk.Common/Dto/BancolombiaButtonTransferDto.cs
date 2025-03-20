using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto.BancolombiaButtonTransferResponseDto
{
    public class BancolombiaButtonTransferResponseDto
    {
        public BancolombiaButtonTransferResponseData Data { get; set; }
        public Meta Meta { get; set; }
    }

    public class BancolombiaButtonTransferResponseData
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public string Status_Message { get; set; }
        public string Bank_Account_Type { get; set; }
        public string Bank_Account_Last_Four { get; set; }
        public string Redirect_Url { get; set; }
        public string Authorization_Url { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }

    public class Meta
    {

    }
}
