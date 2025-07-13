using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Util;
using Util.DTOs.ApiResponse;
using Util.DTOs.DocumentCategoryDTOs;
using Util.DTOs.DocumentTagDTOs;
using Util.DTOs.LawDocumentDTOs;
using Util.Paginated;
using TrafficLawDocumentRazor.Services;
using System.Drawing.Printing;

namespace TrafficLawDocumentRazor.Pages.LawDocumentPage
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ILawDocumentsApiService _lawDocumentsApiService;

        public IndexModel(ILogger<IndexModel> logger, ILawDocumentsApiService lawDocumentsApiService)
        {
            _logger = logger;
            _lawDocumentsApiService = lawDocumentsApiService;
        }

        public PaginatedList<GetLawDocumentDTO> LawDocumentList { get; set; } = new PaginatedList<GetLawDocumentDTO> { Items = new List<GetLawDocumentDTO>() };

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        // Search property
        [BindProperty(SupportsGet = true)] public string? TitleSearch { get; set; }
        [BindProperty(SupportsGet = true)] public string? DocumentCodeSearch { get; set; }
        [BindProperty(SupportsGet = true)] public string? CategoryNameSearch { get; set; }



        public async Task OnGetAsync()
        {
            try
            {
                //LawDocumentList = await _lawDocumentsApiService.GetLawDocumentsAsync(PageIndex, PageSize);
                LawDocumentList = await _lawDocumentsApiService.GetLawDocumentsAsync(
                    PageIndex, PageSize,
                    TitleSearch, DocumentCodeSearch, CategoryNameSearch
                );

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading law documents data");
                LawDocumentList = new PaginatedList<GetLawDocumentDTO> { Items = new List<GetLawDocumentDTO>() };
            }
        }
    }
}
