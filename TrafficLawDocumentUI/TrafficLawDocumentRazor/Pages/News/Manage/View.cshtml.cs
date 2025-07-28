using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrafficLawDocumentRazor.Services;
using Util.DTOs.NewsDTOs;

namespace TrafficLawDocumentRazor.Pages.News.Manage
{
    public class ViewModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly INewsApiService _newsApiService;

        public ViewModel(ILogger<IndexModel> logger, INewsApiService newsApiService)
        {
            _logger = logger;
            _newsApiService = newsApiService;
        }

        public GetNewsDTO? News { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid news ID provided: {NewsId}", id);
                TempData["ErrorMessage"] = "Invalid news ID provided.";
                return BadRequest();
            }
            
            try
            {
                _logger.LogInformation("Attempting to load news with ID: {NewsId}", id);


                News = await _newsApiService.GetNewsByIdAsync(id);

                if (News == null)
                {
                    _logger.LogWarning("News with ID {NewsId} not found", id);
                    TempData["ErrorMessage"] = "News not found.";
                    return NotFound();
                }

                _logger.LogInformation("Successfully loaded news with ID: {NewsId}", id);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading news with ID: {NewsId}", id);
                TempData["ErrorMessage"] = $"An error occurred while loading the news article: {ex.Message}";
                return RedirectToPage("/Error");
            }
        }
    }
}