using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Util;
using Util.DTOs.ApiResponse;
using Util.DTOs.DocumentTagDTOs;
using Util.Paginated;

namespace TrafficLawDocumentRazor.Pages.DocumentTagPage
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public string currentUserRole { get; set; } = default!;
        public PaginatedList<GetDocumentTagDTO> DocumentTag { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1, int pageSize = 10, string? name = null, string? parentName = null)
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (currentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }

            // Get query parameters for pagination
            if (Request.Query.ContainsKey("pageIndex"))
            {
                pageNumber = int.Parse(Request.Query["pageNumber"]!);
            }
            if (Request.Query.ContainsKey("pageSize"))
            {
                pageSize = int.Parse(Request.Query["pageSize"]!);
            }

            // Construct the API URL with query parameters for pagination
            string apiUrl = $"document-tags?pageIndex={pageNumber}&pageSize={pageSize}";

            // Append additional query parameters if they are provided
            if (!string.IsNullOrWhiteSpace(name))
            {
                apiUrl += $"&nameSearch={Uri.EscapeDataString(name)}";
            }

            if (!string.IsNullOrWhiteSpace(parentName))
            {
                apiUrl += $"&parentNameSearch={Uri.EscapeDataString(parentName)}";
            }

            // Fetch tags
            var tagResponse = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentTagDTO>>>(apiUrl);

            if (tagResponse?.Data != null)
            {
                DocumentTag = tagResponse.Data;
            }
            else
            {
                // If the response is null or does not contain data, initialize an empty PaginatedList
                DocumentTag = new PaginatedList<GetDocumentTagDTO>
                {
                    Items = new List<GetDocumentTagDTO>(),
                    PageNumber = pageNumber,
                    TotalPages = 0,
                    TotalCount = 0,
                    PageSize = pageSize
                };
            }

            return Page();
        }
    }
}
