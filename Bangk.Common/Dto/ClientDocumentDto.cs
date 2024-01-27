using Bangk.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Bangk.Common.Dto
{
    public class ClientDocumentDto
    {
        public int ClientId { get; set; }
        public string IdentificationName { get; } = "Cédula";
        public DateTime CreateIdentificationDate { get; set; }
        public string IdentificationUrl { get; set; }
        public List<ClientDocumentListDto> ClientDocumentListDtos { get; set; }
    }

    public class ClientDocumentListDto
    {
        public int DocumentType {  get; set; }
        public List<DocumentByTypeDto> DocumentByTypeDtos { get; set; }
    }

    public class DocumentByTypeDto
    {
        public string DocumentUrl { get; set; }
        public string DocumentName  { get; set; }
        public DateTime CreateDocumentDate { get; set; }
    }
}
