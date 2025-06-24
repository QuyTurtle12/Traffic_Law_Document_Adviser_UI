using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrafficLawDocumentRazor.Services;
using Util.DTOs.NewsDTOs;

namespace TrafficLawDocumentRazor.Pages
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

        public List<GetNewsDTO> LatestNews { get; set; } = new List<GetNewsDTO>();

        public async Task OnGetAsync()
        {
            try
            {
                // Get the latest 3 news articles for the homepage
                var newsResult = await _newsApiService.GetNewsAsync(1, 3);
                LatestNews = newsResult.Items?.ToList() ?? new List<GetNewsDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading latest news for homepage");
                LatestNews = new List<GetNewsDTO>();
            }
        }
    }
}
