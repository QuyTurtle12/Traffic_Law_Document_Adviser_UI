using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrafficLawDocumentRazor.Services;
using Util.DTOs.NewsDTOs;

namespace TrafficLawDocumentRazor.Pages.News.Manage
{
    public class DeleteModel : PageModel
    {
        private readonly ILogger<DeleteModel> _logger;
        private readonly INewsApiService _newsApiService;

        public DeleteModel(ILogger<DeleteModel> logger, INewsApiService newsApiService)
        {
            _logger = logger;
            _newsApiService = newsApiService;
        }

        [BindProperty]
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
                _logger.LogError(ex, "Error loading news for deletion");
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            try
            {
                var success = await _newsApiService.DeleteNewsAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "News article deleted successfully!";
                    return RedirectToPage("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete the news article. Please try again.";
                    return RedirectToPage("Index");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting news article");
                TempData["ErrorMessage"] = "An error occurred while deleting the news article. Please try again.";
                return RedirectToPage("Index");
            }
        }
    }
} 