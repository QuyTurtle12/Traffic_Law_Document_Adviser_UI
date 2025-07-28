using System.Net.Http.Headers;
using Util.DTOs.ApiResponse;
using Util.DTOs.LawDocumentDTOs;
using Util.Paginated;

namespace TrafficLawDocumentRazor.Services
{
    public interface ILawDocumentsApiService
    {
        // Task<PaginatedList<GetLawDocumentDTO>> GetLawDocumentsAsync(int pageIndex, int pageSize);
        Task<PaginatedList<GetLawDocumentDTO>> GetLawDocumentsAsync(int pageIndex, int pageSize, string? titleSearch = null,
            string? documentCodeSearch = null,
            string? categoryNameSearch = null);
    }
    public class LawDocumentsApiService : ILawDocumentsApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LawDocumentsApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginatedList<GetLawDocumentDTO>> GetLawDocumentsAsync(int pageIndex, int pageSize, string? titleSearch = null,
            string? documentCodeSearch = null,
            string? categoryNameSearch = null)
        {
            var baseUrl = _configuration["ApiSettings:BaseUrl"];
            var token = _httpContextAccessor.HttpContext?.Request?.Cookies["access_token"]; // Or session, or user claims

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var url = $"{baseUrl}law-documents?pageIndex={pageIndex}&pageSize={pageSize}";

            if (!string.IsNullOrEmpty(titleSearch))
                url += $"&titleSearch={Uri.EscapeDataString(titleSearch)}";

            if (!string.IsNullOrEmpty(documentCodeSearch))
                url += $"&documentCodeSearch={Uri.EscapeDataString(documentCodeSearch)}";

            if (!string.IsNullOrEmpty(categoryNameSearch))
                url += $"&categoryNameSearch={Uri.EscapeDataString(categoryNameSearch)}";

            //if (!string.IsNullOrEmpty(categoryNameSearch))
            //    url += $"&expertVerificationSearch={Uri.EscapeDataString(expertVerificationSearch)}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetLawDocumentDTO>>>(url);

            return response?.Data ?? new PaginatedList<GetLawDocumentDTO>();
        }


    }
}
