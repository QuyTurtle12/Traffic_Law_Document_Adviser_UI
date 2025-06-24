using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrafficLawDocumentRazor.Services;
using Util.DTOs.LawDocumentDTOs;
using Util.Paginated;

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

        public async Task OnGetAsync()
        {
            try
            {
                LawDocumentList = await _lawDocumentsApiService.GetLawDocumentsAsync(PageIndex, PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading law documents data");
                LawDocumentList = new PaginatedList<GetLawDocumentDTO> { Items = new List<GetLawDocumentDTO>() };
            }
        }
    }
}
