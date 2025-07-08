using System.Net.Http.Headers;
using Util.DTOs.ApiResponse;
using Util.DTOs.UserDTOs;
using Util.Paginated;
using System.Net.Http.Json;

namespace TrafficLawDocumentRazor.Services
{
    public interface IUserApiService
    {
        Task<PaginatedList<UserDTO>> GetUsersAsync(int pageIndex, int pageSize);
        Task<ApiResponse<UserDTO>> CreateUserAsync(CreateUserDTO dto);
    }
    public class UserApiService: IUserApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginatedList<UserDTO>> GetUsersAsync(int pageIndex, int pageSize)
        {
            var baseUrl = _configuration["ApiSettings:BaseUrl"];
            var token = _httpContextAccessor.HttpContext?.Request?.Cookies["AccessToken"]; // Or session, or user claims

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var url = $"{baseUrl}users?pageIndex={pageIndex}&pageSize={pageSize}";

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<UserDTO>>>(url);

            return new PaginatedList<UserDTO>
            {
                Items = response?.Data ?? new List<UserDTO>(),
                PageNumber = pageIndex,
                PageSize = pageSize,
                TotalCount = response?.Data?.Count ?? 0
            };
        }

        public async Task<ApiResponse<UserDTO>> CreateUserAsync(CreateUserDTO dto)
        {
            var baseUrl = _configuration["ApiSettings:BaseUrl"];
            var token = _httpContextAccessor.HttpContext?.Request?.Cookies["AccessToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var url = $"{baseUrl}users";
            var response = await _httpClient.PostAsJsonAsync(url, dto);
            response.EnsureSuccessStatusCode();
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDTO>>();
            return apiResponse!;
        }
    }
}
