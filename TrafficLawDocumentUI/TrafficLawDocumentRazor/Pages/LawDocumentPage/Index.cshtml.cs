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

        [BindProperty(SupportsGet = true)]
        public string? SearchKeyword { get; set; }
        public PaginatedList<GetLawDocumentDTO> LawDocumentList { get; set; } = new PaginatedList<GetLawDocumentDTO> { Items = new List<GetLawDocumentDTO>() };

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;



        public async Task OnGetAsync()
        {
            try
            {
                //LawDocumentList = await _lawDocumentsApiService.GetLawDocumentsAsync(PageIndex, PageSize);
                var result = await _lawDocumentsApiService.GetLawDocumentsAsync(PageIndex, PageSize, SearchKeyword);
                LawDocumentList = result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading law documents data");
                LawDocumentList = new PaginatedList<GetLawDocumentDTO> { Items = new List<GetLawDocumentDTO>() };
            }
        }
    }
}
