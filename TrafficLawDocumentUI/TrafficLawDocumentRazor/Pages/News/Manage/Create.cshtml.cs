using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TrafficLawDocumentRazor.Services;
using Util.DTOs.NewsDTOs;

namespace TrafficLawDocumentRazor.Pages.News.Manage
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _logger;
        private readonly INewsApiService _newsApiService;

        public CreateModel(ILogger<CreateModel> logger, INewsApiService newsApiService)
        {
            _logger = logger;
            _newsApiService = newsApiService;
        }

        [BindProperty]
        public CreateNewsInput News { get; set; } = new CreateNewsInput();

        public void OnGet()
        {
            // Initialize with default values
            News.PublishedDate = DateTime.Now;
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

                var createdNews = await _newsApiService.CreateNewsAsync(addNewsDto);

                if (createdNews != null)
                {
                    TempData["SuccessMessage"] = "News article created successfully!";
                    return RedirectToPage("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create the news article. Please try again.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating news article");
                ModelState.AddModelError("", "An error occurred while creating the news article. Please try again.");
                return Page();
            }
        }
    }

    public class CreateNewsInput
    {
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