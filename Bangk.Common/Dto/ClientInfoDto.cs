using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bangk.Common.Dto
{
    public class ClientInfoDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string DocumentType {  get; set; }
        public string IdentificationNumber { get; set; }
        public int IdClient { get; set; }
        public string Email {  get; set; }
        public int CountryCode { get; set; }
        public string  PhoneNumber {  get; set; }
        public DateTime BirthDate { get; set; }
        public string StateName { get; set; }
        public string GenderName {  get; set; }
        public DateTime DocumentIssueDate { get; set; }

    }

    public class ClientUpdateInfoDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int DocumentTypeId { get; set; }
        public string IdentificationNumber { get; set; }
        public string Email { get; set; }
        public int CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public int GenderId { get; set; }
        public DateTime DocumentIssueDate { get; set; }
    }

}
