using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TrafficLawDocumentRazor.Services;
using Util.DTOs.NewsDTOs;

namespace TrafficLawDocumentRazor.Pages.News.Manage
{
    public class EditModel : PageModel
    {
        private readonly ILogger<EditModel> _logger;
        private readonly INewsApiService _newsApiService;

        public EditModel(ILogger<EditModel> logger, INewsApiService newsApiService)
        {
            _logger = logger;
            _newsApiService = newsApiService;
        }

        [BindProperty]
        public EditNewsInput News { get; set; } = new EditNewsInput();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var news = await _newsApiService.GetNewsByIdAsync(id);
                
                if (news == null)
                {
                    return NotFound();
                }

                News = new EditNewsInput
                {
                    Id = news.Id,
                    Title = news.Title ?? string.Empty,
                    Content = news.Content ?? string.Empty,
                    Author = news.Author,
                    PublishedDate = news.PublishedDate,
                    ImageUrl = news.ImageUrl,
                    EmbeddedUrl = news.EmbeddedUrl
                };

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading news for editing");
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var addNewsDto = new AddNewsDTO
                {
                    Title = News.Title,
                    Content = News.Content,
                    Author = News.Author,
                    PublishedDate = News.PublishedDate,
                    ImageUrl = News.ImageUrl,
                    EmbeddedUrl = News.EmbeddedUrl
                };

                var updatedNews = await _newsApiService.UpdateNewsAsync(News.Id, addNewsDto);

                if (updatedNews != null)
                {
                    TempData["SuccessMessage"] = "News article updated successfully!";
                    return RedirectToPage("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update the news article. Please try again.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating news article");
                ModelState.AddModelError("", "An error occurred while updating the news article. Please try again.");
                return Page();
            }
        }
    }

    public class EditNewsInput
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
        public string Content { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Author name cannot exceed 100 characters")]
        public string? Author { get; set; }

        [Required(ErrorMessage = "Published date is required")]
        public DateTime PublishedDate { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? ImageUrl { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? EmbeddedUrl { get; set; }
    }
} 