using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObject;
using Microsoft.EntityFrameworkCore;
using Util.DTOs.NewsDTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace TrafficLawDocumentRazor.Pages.News.Manage
{
    public class ViewModel : PageModel
    {
        private readonly TrafficLawDocumentDbContext _context;
        private readonly ILogger<ViewModel> _logger;

        public ViewModel(TrafficLawDocumentDbContext context, ILogger<ViewModel> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                
                // Check if the database is accessible
                if (!_context.Database.CanConnect())
                {
                    _logger.LogError("Cannot connect to database");
                    TempData["ErrorMessage"] = "Database connection error.";
                    return StatusCode(500);
                }

                News = await _context.News
                    .Where(n => n.Id == id && n.DeletedTime == null)
                    .Select(n => new GetNewsDTO
                    {
                        Id = n.Id,
                        Title = n.Title,
                        Content = n.Content,
                        PublishedDate = n.PublishedDate,
                        Author = n.Author,
                        ImageUrl = n.ImageUrl,
                        EmbeddedUrl = n.EmbeddedUrl,
                        CreatedTime = n.CreatedTime,
                        LastUpdatedTime = n.LastUpdatedTime,
                        UserId = n.UserId
                    })
                    .FirstOrDefaultAsync();

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