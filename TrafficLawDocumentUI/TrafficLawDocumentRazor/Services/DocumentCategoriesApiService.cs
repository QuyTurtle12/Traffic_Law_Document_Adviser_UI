using System.Net.Http.Headers;
using Util.DTOs.ApiResponse;
using Util.DTOs.DocumentCategoryDTOs;
using Util.DTOs.LawDocumentDTOs;
using Util.Paginated;

namespace TrafficLawDocumentRazor.Services
{
    public interface IDocumentCategoriesApiService
    {
        Task<PaginatedList<GetDocumentCategoryDTO>> GetAllCategoriesAsync(int pageIndex = 1, int pageSize = 100);
    }

    public class DocumentCategoriesApiService : IDocumentCategoriesApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DocumentCategoriesApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginatedList<GetDocumentCategoryDTO>> GetAllCategoriesAsync(int pageIndex = 1, int pageSize = 100)
        {
            var baseUrl = _configuration["ApiSettings:BaseUrl"];
            var token = _httpContextAccessor.HttpContext?.Request?.Cookies["AccessToken"];

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var url = $"{baseUrl}document-categories?pageIndex={pageIndex}&pageSize={pageSize}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentCategoryDTO>>>(url);

            return response?.Data ?? new PaginatedList<GetDocumentCategoryDTO>();
        }
    }
}
