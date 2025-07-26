using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Util.DTOs.ApiResponse;
using Util.DTOs.LawDocumentDTOs;
using Util.Paginated;

namespace TrafficLawDocumentRazor.Pages.LawDocumentPage
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DetailsModel(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("API");
            _configuration = configuration;
        }


        public LawDocument LawDocument { get; set; } = default!;
        public string ApiBaseUrl { get; private set; } = "";

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            var currentDocument = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetLawDocumentDTO>>>($"law-documents?pageIndex=1&pageSize=1&idSearch={id}");

            if (currentDocument?.Data?.Items == null || !currentDocument.Data.Items.Any())
                return NotFound();

            var document = currentDocument.Data.Items.First();

            LawDocument = new LawDocument
            {
                Id = document.Id,
                Title = document.Title,
                DocumentCode = document.DocumentCode,
                CategoryId = document.CategoryId,
                FilePath = document.FilePath,
                LinkPath = document.LinkPath,
                ExpertVerification = document.ExpertVerification
            };

            ApiBaseUrl = (_configuration["ApiSettings:BaseUrl"] ?? "")
                         .TrimEnd('/');

            return Page();
        }
    }
}
