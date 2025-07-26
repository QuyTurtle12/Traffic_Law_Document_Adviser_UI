
using System.ComponentModel.DataAnnotations;

namespace Util.DTOs.LawDocumentDTOs
{
    public class BaseLawDocumentDTO
    {
        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Document code is required")]
        public string? DocumentCode { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public Guid? CategoryId { get; set; }
        public string? FilePath { get; set; }
        public string? LinkPath { get; set; }
        public bool ExpertVerification { get; set; } = false;
    }
}
