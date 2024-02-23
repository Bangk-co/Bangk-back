using System;
using System.Collections.Generic;

namespace Bangk.Common.Dto
{
    public class ClientDocumentDto
    {
        public int ClientId { get; set; }
        public string IdentificationName { get; } = "Cédula";
        public DateTime CreateIdentificationDate { get; set; }
        public int DocumentIdentificationId { get; set; }
        public List<ClientDocumentListDto> ClientDocumentListDtos { get; set; }
    }

    public class ClientDocumentListDto
    {
        public int DocumentType {  get; set; }
        public List<DocumentByTypeDto> DocumentByTypeDtos { get; set; }
    }

    public class DocumentByTypeDto
    {
        public int DocumentId { get; set; }
        public string DocumentName  { get; set; }
        public DateTime CreateDocumentDate { get; set; }
    }
}
