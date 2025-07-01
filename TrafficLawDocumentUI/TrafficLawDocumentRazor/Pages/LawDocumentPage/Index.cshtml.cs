using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Util;
using Util.DTOs.ApiResponse;
using Util.DTOs.DocumentCategoryDTOs;
using Util.DTOs.DocumentTagDTOs;
using Util.DTOs.LawDocumentDTOs;
using Util.Paginated;

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
        public List<SelectListItem> Tags { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Title { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? DocumentCode { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? CategoryName { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool? ExpertVerification { get; set; }

        [BindProperty(SupportsGet = true)]
        public string[]? TagIds { get; set; }

        public async Task OnGetAsync(int pageNumber = 1, int pageSize = 10)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenStore.Token);

            // Fetch categories
            var catResponse = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentCategoryDTO>>>(
                "document-categories?pageIndex=1&pageSize=100");
            if (catResponse?.Data?.Items != null)
                Categories = catResponse.Data.Items.ToList();

            // Fetch tags
            var tagResponse = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentTagDTO>>>(
                "document-tags?pageIndex=1&pageSize=100");
            if (tagResponse?.Data?.Items != null)
                Tags = tagResponse.Data.Items
                    .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name })
                    .ToList();

            var queryParams = new List<string>
        {
            $"pageIndex={pageNumber}",
            $"pageSize={pageSize}"
        };

            if (!string.IsNullOrEmpty(Title))
            {
                queryParams.Add($"titleSearch={Uri.EscapeDataString(Title)}");
            }

            if (!string.IsNullOrEmpty(DocumentCode))
            {
                queryParams.Add($"documentCodeSearch={Uri.EscapeDataString(DocumentCode)}");
            }

            if (!string.IsNullOrEmpty(CategoryName))
            {
                queryParams.Add($"categoryNameSearch={Uri.EscapeDataString(CategoryName)}");
            }

            if (ExpertVerification.HasValue)
            {
                queryParams.Add($"expertVerificationSearch={ExpertVerification.Value}");
            }

            if (TagIds != null && TagIds.Any())
            {
                foreach (var tagId in TagIds)
                {
                    queryParams.Add($"tagIds={tagId}");
                }
            }

            string apiUrl = $"law-documents?{string.Join("&", queryParams)}";

            var apiResponse = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetLawDocumentDTO>>>(apiUrl);

            if (apiResponse?.Data != null)
            {
                LawDocument = apiResponse.Data;
            }
        }
    }
}
