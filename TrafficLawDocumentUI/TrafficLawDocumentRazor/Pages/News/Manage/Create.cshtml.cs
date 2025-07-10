using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Net.Http.Json;
using TrafficLawDocumentRazor.Services;
using Util.DTOs.NewsDTOs;

namespace TrafficLawDocumentRazor.Pages.News.Manage
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }
        public string CurrentUserRole { get; set; } = default!;
        public string CurrentUserId { get; private set; }
        [BindProperty]
        public CreateNewsInput News { get; set; } = new CreateNewsInput();
        public async Task<IActionResult> OnGetAsync()
        {
            CurrentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            if (CurrentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }
            News.PublishedDate = DateTime.Now;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            CurrentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                 ?? User.FindFirstValue("sub");
            if (CurrentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var addNewsDto = new Util.DTOs.NewsDTOs.AddNewsDTO
                {
                    UserId = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var userId) ? userId : (Guid?)null,
                    Title = News.Title,
                    Content = News.Content,
                    Author = News.Author,
                    PublishedDate = News.PublishedDate,
                    ImageUrl = News.ImageUrl,
                    EmbeddedUrl = News.EmbeddedUrl
                };
                var response = await _httpClient.PostAsJsonAsync("news", addNewsDto);
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Failed to create the news article. Please try again.");
                    return Page();
                }
                TempData["SuccessMessage"] = "News article created successfully!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
            }
            ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
            return RedirectToPage("Index");
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