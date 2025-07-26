using Util.DTOs.DocumentTagMapDTOs;

namespace Util.DTOs.LawDocumentDTOs
{
    public class UpdateLawDocumentDTO : BaseLawDocumentDTO
    {
        public IEnumerable<AddDocumentTagMapDTO>? TagList { get; set; }
    }
}
