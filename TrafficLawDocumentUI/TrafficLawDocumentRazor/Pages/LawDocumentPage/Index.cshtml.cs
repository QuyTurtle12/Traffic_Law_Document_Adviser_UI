using Microsoft.AspNetCore.Mvc.RazorPages;
using Util.Paginated;
using Util.DTOs.LawDocumentDTOs;
using Util.DTOs.ApiResponse;

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

        public async Task OnGetAsync(int pageNumber = 1, int pageSize = 1)
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

            // Fetch paginated law documents from the API
            var apiResponse = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetLawDocumentDTO>>>($"/api/law-documents?pageIndex={pageNumber}&pageSize={pageSize}");

            // Check if the response is not null before assigning it to the LawDocument property
            if (apiResponse != null)
            {
                LawDocument = apiResponse.Data;
            }
        }
    }
}
