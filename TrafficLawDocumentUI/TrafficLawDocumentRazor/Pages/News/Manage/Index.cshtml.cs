using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrafficLawDocumentRazor.Services;
using Util.DTOs.NewsDTOs;
using Util.Paginated;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net.Http.Json;

namespace TrafficLawDocumentRazor.Pages.News.Manage
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly INewsApiService _newsApiService;
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(ILogger<IndexModel> logger, INewsApiService newsApiService, IHttpClientFactory httpClientFactory)
{
    _logger = logger;
    _newsApiService = newsApiService;
    _httpClientFactory = httpClientFactory;
}

        public PaginatedList<GetNewsDTO> NewsList { get; set; } = new PaginatedList<GetNewsDTO> { Items = new List<GetNewsDTO>() };
        
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 20;

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

        public string CurrentUserRole { get; set; } = default!;

        public List<SyncNewsItem>? SyncedArticles { get; set; }
        public string? SyncMessage { get; set; }
        public string? SyncStatus { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            
            try
            {
                CurrentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
                if (CurrentUserRole != "Staff")
                {
                    return RedirectToPage("/Index");
                }
                NewsList = await _newsApiService.GetNewsAsync(PageIndex, PageSize, TitleFilter, AuthorFilter, ContentFilter, StartDate, EndDate);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading news management data");
                NewsList = new PaginatedList<GetNewsDTO> { Items = new List<GetNewsDTO>() };
                TempData["ErrorMessage"] = ex.Message;
            }
            if (TempData.ContainsKey("SyncMessage"))
            {
                SyncMessage = TempData["SyncMessage"] as string;
                SyncStatus = TempData["SyncStatus"] as string;
                if (TempData.ContainsKey("SyncedArticles"))
                {
                    var json = TempData["SyncedArticles"] as string;
                    if (!string.IsNullOrEmpty(json))
                    {
                        try
                        {
                            SyncedArticles = System.Text.Json.JsonSerializer.Deserialize<List<SyncNewsItem>>(json);
                        }
                        catch { SyncedArticles = null; }
                    }
                }
            }
            return Page();
        }

        public class SyncNewsApiResponse
        {
            public List<SyncNewsItem> Data { get; set; } = new();
            public string? AdditionalData { get; set; }
            public string? Message { get; set; }
            public int StatusCode { get; set; }
            public string? Code { get; set; }
        }
        public class SyncNewsItem
        {
            public Guid Id { get; set; }
            public DateTime PublishedDate { get; set; }
            public string? Author { get; set; }
            public string? ImageUrl { get; set; }
            public string? EmbeddedUrl { get; set; }
            public long EmbeddedNewsId { get; set; }
            public DateTime CreatedTime { get; set; }
            public DateTime LastUpdatedTime { get; set; }
            public Guid? UserId { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Content { get; set; } = string.Empty;
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OnPostSyncNewsAsync()
        {
            CurrentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            if (CurrentUserRole != "Staff")
            {
                TempData["SyncStatus"] = "error";
                TempData["SyncMessage"] = "You do not have permission to sync news.";
                return RedirectToPage();
            }
            try
            {
                var client = _httpClientFactory.CreateClient("API");
                var response = await client.PostAsync("news/sync", null);
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var apiResult = System.Text.Json.JsonSerializer.Deserialize<SyncNewsApiResponse>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        TempData["SyncStatus"] = "success";
                        TempData["SyncMessage"] = apiResult?.AdditionalData ?? "News synced successfully!";
                        TempData["SyncedArticles"] = System.Text.Json.JsonSerializer.Serialize(apiResult?.Data ?? new List<SyncNewsItem>());
                        return RedirectToPage();
                    }
                    catch
                    {
                        TempData["SyncStatus"] = "success";
                        TempData["SyncMessage"] = "News synced successfully!";
                        TempData["SyncedArticles"] = "[]";
                        return RedirectToPage();
                    }
                }
                else
                {
                    string errorMsg = "Failed to sync news. Please try again.";
                    try
                    {
                        var apiError = System.Text.Json.JsonSerializer.Deserialize<SyncNewsApiResponse>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (!string.IsNullOrEmpty(apiError?.Message))
                        {
                            errorMsg = apiError.Message;
                        }
                        TempData["SyncStatus"] = "error";
                        TempData["SyncMessage"] = errorMsg;
                        TempData["SyncedArticles"] = "[]";
                        return RedirectToPage();
                    }
                    catch
                    {
                        TempData["SyncStatus"] = "error";
                        TempData["SyncMessage"] = errorMsg;
                        TempData["SyncedArticles"] = "[]";
                        return RedirectToPage();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing news");
                TempData["SyncStatus"] = "error";
                TempData["SyncMessage"] = "An error occurred while syncing news.";
                TempData["SyncedArticles"] = "[]";
                return RedirectToPage();
            }
        }
    }
} 