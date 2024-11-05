using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class ClientClientifyDto
    {
        public string contact_source;

        public string owner;
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string status { get; set; }
        public string phone { get; set; }
        public string gdpr_acceptance_date { get; set; }
        public bool gdpr_acceptance => true;
        public string birthday { get; set; }
        public string taxpayer_identification_number { get; set; }
        public List<CustomFieldsDto> custom_fields { get; set; }
    }

    public class CustomFieldsDto
    {
        public string field { get; set; }
        public string value { get; set; }
    }

    public class DealClientifyDto
    {
        public string pipeline_desc;
        public string name { get; set; }
        public double amount { get; set; }
        public string currency { get; set; }
        public string contact { get; set; }
        public string pipeline { get; set; }
        public string expected_closed_date { get; set; }
        public string pipeline_stage_desc { get; set; }
        public string owner { get; set; }
        public List<CustomFieldsDto> custom_fields { get; set; }
    }

    public class UpdateStatusClientifyDto
    {
        public string pipeline_stage { get; set; }
        public string pipeline_stage_desc { get; set; }
    }

    public class UpdateStateClientifyDto
    {
        public int status { get; set; }
    }
}


