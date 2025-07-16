using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Security.Claims;
using Util.DTOs.NewsDTOs;
using Util.DTOs.ApiResponse;

namespace TrafficLawDocumentRazor.Pages.News.Manage
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }
        public string CurrentUserRole { get; set; } = default!;
        public string? ErrorMessage { get; set; }
        [BindProperty]
        public EditNewsInput News { get; set; } = new EditNewsInput();
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            CurrentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            if (CurrentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<GetNewsDTO>>($"news/{id}");
            var news = response?.Data;
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
        public async Task<IActionResult> OnPostAsync()
        {
            CurrentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
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
                    Title = News.Title,
                    Content = News.Content,
                    Author = News.Author,
                    PublishedDate = News.PublishedDate,
                    ImageUrl = News.ImageUrl,
                    EmbeddedUrl = News.EmbeddedUrl
                };
                var response = await _httpClient.PutAsJsonAsync($"news/{News.Id}", addNewsDto);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorObj = System.Text.Json.JsonSerializer.Deserialize<Util.DTOs.ApiResponse.ApiErrorResponse>(errorContent);
                    ErrorMessage = errorObj?.Message ?? "Failed to update the news article. Please try again.";
                    return Page();
                }
                TempData["SuccessMessage"] = "News article updated successfully!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
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