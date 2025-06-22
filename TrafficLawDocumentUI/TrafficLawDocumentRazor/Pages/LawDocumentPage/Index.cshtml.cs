using Microsoft.AspNetCore.Mvc.RazorPages;
using Util.Paginated;
using Util.DTOs.LawDocumentDTOs;
using Util.DTOs.ApiResponse;
using Util.DTOs.DocumentCategoryDTOs;

namespace TrafficLawDocumentRazor.Pages.LawDocumentPage
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public PaginatedList<GetLawDocumentDTO> LawDocument { get;set; } = default!;
        public List<GetDocumentCategoryDTO> Categories { get; set; } = new();

        public async Task OnGetAsync(int pageNumber = 1, int pageSize = 10, string? title = null, string? documentCode = null, string? categoryName = null, bool? expertVerification = null)
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

            // Fetch categories
            var catResponse = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentCategoryDTO>>>(
                "/api/document-categories?pageIndex=1&pageSize=100");
            if (catResponse?.Data?.Items != null)
                Categories = catResponse.Data.Items.ToList();

            // Construct the API URL with query parameters for pagination and filtering
            string apiUrl = $"/api/law-documents?pageIndex={pageNumber}&pageSize={pageSize}";

            // Append additional query parameters if they are provided
            if (!string.IsNullOrEmpty(title))
            {
                apiUrl += $"&titleSearch={Uri.EscapeDataString(title)}";
            }

            if (!string.IsNullOrEmpty(documentCode))
            {
                apiUrl += $"&documentCodeSearch={Uri.EscapeDataString(documentCode)}";
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                apiUrl += $"&categoryNameSearch={Uri.EscapeDataString(categoryName)}";
            }

            if (expertVerification.HasValue)
            {
                apiUrl += $"&expertVerificationSearch={expertVerification.Value}";
            }

            // Fetch paginated law documents from the API
            var apiResponse = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetLawDocumentDTO>>>(apiUrl);

            // Check if the response is not null before assigning it to the LawDocument property
            if (apiResponse != null)
            {
                LawDocument = apiResponse.Data;
            }
        }
    }
}
