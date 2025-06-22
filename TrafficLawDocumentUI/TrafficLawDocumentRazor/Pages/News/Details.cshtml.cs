using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrafficLawDocumentRazor.Services;
using Util.DTOs.NewsDTOs;

namespace TrafficLawDocumentRazor.Pages.News
{
    public class DetailsModel : PageModel
    {
        private readonly ILogger<DetailsModel> _logger;
        private readonly INewsApiService _newsApiService;

        public DetailsModel(ILogger<DetailsModel> logger, INewsApiService newsApiService)
        {
            _logger = logger;
            _newsApiService = newsApiService;
        }

        public GetNewsDTO? News { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                News = await _newsApiService.GetNewsByIdAsync(id);
                
                if (News == null)
                {
                    return NotFound();
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading news details");
                return NotFound();
            }
        }
    }
} 