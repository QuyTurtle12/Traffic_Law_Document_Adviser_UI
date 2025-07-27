using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrafficLawDocumentRazor.Services;
using Util.DTOs.NewsDTOs;
using Util.Paginated;

namespace TrafficLawDocumentRazor.Pages.News
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
        public int PageSize { get; set; } = 9;

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public string? TitleFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? AuthorFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ContentFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Reset to page 1 when filters are applied
                if (!string.IsNullOrEmpty(TitleFilter) || !string.IsNullOrEmpty(AuthorFilter) || 
                    !string.IsNullOrEmpty(ContentFilter) || StartDate.HasValue || EndDate.HasValue)
                {
                    PageIndex = 1;
                }

                NewsList = await _newsApiService.GetNewsAsync(PageIndex, PageSize, TitleFilter, AuthorFilter, ContentFilter, StartDate, EndDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading news data");
                NewsList = new PaginatedList<GetNewsDTO> { Items = new List<GetNewsDTO>() };
            }
        }
    }
} 