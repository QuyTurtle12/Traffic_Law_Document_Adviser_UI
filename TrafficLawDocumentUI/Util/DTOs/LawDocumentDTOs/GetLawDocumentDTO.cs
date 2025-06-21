using Util.DTOs.DocumentTagMapDTOs;

namespace Util.DTOs.LawDocumentDTOs
{
    public class GetLawDocumentDTO : BaseLawDocumentDTO
    {
        public Guid Id { get; set; }
        public string? CategoryName { get; set; }
        public IEnumerable<GetDocumentTagMapDTO>? TagList { get; set; }
    }
}
