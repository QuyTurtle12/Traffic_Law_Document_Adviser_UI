using Microsoft.AspNetCore.Mvc.RazorPages;
using Util;
using Util.DTOs.ApiResponse;
using Util.DTOs.DocumentCategoryDTOs;
using Util.Paginated;

namespace TrafficLawDocumentRazor.Pages.DocumentCategoryPage
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public PaginatedList<GetDocumentCategoryDTO> DocumentCategory { get;set; } = default!;

        public async Task OnGetAsync(int pageNumber = 1, int pageSize = 10, string? name = null)
        {
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
            string apiUrl = $"/api/document-categories?pageIndex={pageNumber}&pageSize={pageSize}";

            // Append additional query parameters if they are provided
            if (!string.IsNullOrWhiteSpace(name))
            {
                apiUrl += $"&nameSearch={Uri.EscapeDataString(name)}";
            }

            // Set the authorization header with the JWT token
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenStore.Token);

            // Fetch tags
            var catResponse = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentCategoryDTO>>>(apiUrl);

            if (catResponse?.Data != null)
            {
                DocumentCategory = catResponse.Data;
            }
            else
            {
                // If the response is null or does not contain data, initialize an empty PaginatedList
                DocumentCategory = new PaginatedList<GetDocumentCategoryDTO>
                {
                    Items = new List<GetDocumentCategoryDTO>(),
                    PageNumber = pageNumber,
                    TotalPages = 0,
                    TotalCount = 0,
                    PageSize = pageSize
                };
            }

        }
    }
}
