using Util.DTOs.DocumentTagMapDTOs;

namespace Util.DTOs.LawDocumentDTOs
{
    public class AddLawDocumentDTO : BaseLawDocumentDTO
    {
        public IEnumerable<AddDocumentTagMapDTO>? TagList { get; set; }
    }
}
