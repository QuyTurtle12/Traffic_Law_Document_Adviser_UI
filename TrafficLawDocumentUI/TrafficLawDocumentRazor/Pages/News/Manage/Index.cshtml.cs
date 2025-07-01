using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrafficLawDocumentRazor.Services;
using Util.DTOs.NewsDTOs;
using Util.Paginated;

namespace TrafficLawDocumentRazor.Pages.News.Manage
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly INewsApiService _newsApiService;

        public IndexModel(ILogger<IndexModel> logger, INewsApiService newsApiService)
        {
            _logger = logger;
            _newsApiService = newsApiService;
        }

        public PaginatedList<GetNewsDTO> NewsList { get; set; } = new PaginatedList<GetNewsDTO> { Items = new List<GetNewsDTO>() };
        
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 20;

        public async Task OnGetAsync()
        {
            try
            {
                NewsList = await _newsApiService.GetNewsAsync(PageIndex, PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading news management data");
                NewsList = new PaginatedList<GetNewsDTO> { Items = new List<GetNewsDTO>() };
            }
        }
    }
} 