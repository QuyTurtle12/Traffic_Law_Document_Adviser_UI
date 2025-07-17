using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Util.DTOs.ApiResponse;
using Util.DTOs.UserDTOs;
using Util.Paginated;
using System.Text.Json;

namespace TrafficLawDocumentRazor.Services
{
    public interface IUserApiService
    {
        Task<PaginatedList<UserDTO>> GetUsersAsync(int pageIndex, int pageSize);
        Task<ApiResponse<UserDTO>> CreateUserAsync(CreateUserDTO dto);
        Task<UserDTO?> GetUserByIdAsync(Guid id);
        Task<bool> DeleteUserAsync(Guid id);
        Task<ApiResponse<UserDTO>> UpdateUserAsync(Guid id, UpdateUserDTO dto);
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
            var token = _httpContextAccessor.HttpContext?.User?.FindFirstValue("access_token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var url = $"{baseUrl}users";
            var response = await _httpClient.PostAsJsonAsync(url, dto);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<UserDTO>>(responseContent, jsonOptions);
                return apiResponse!;
            }
            else
            {
                var errorObj = JsonSerializer.Deserialize<ApiErrorResponse>(responseContent);
                return new ApiResponse<UserDTO>
                {
                    StatusCode = (int)response.StatusCode,
                    Code = errorObj?.Code,
                    Message = errorObj?.ErrorMessage ?? errorObj?.Message ?? $"Failed to create user. Status: {response.StatusCode}",
                    Data = default
                };
            }
        }

        public async Task<UserDTO?> GetUserByIdAsync(Guid id)
        {
            try
            {
                var baseUrl = _configuration["ApiSettings:BaseUrl"];
                var token = _httpContextAccessor.HttpContext?.User?.FindFirstValue("access_token");
                
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var url = $"{baseUrl}users/{id}";
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDTO>>();
                    return apiResponse?.Data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            try
            {
                var baseUrl = _configuration["ApiSettings:BaseUrl"];
                var token = _httpContextAccessor.HttpContext?.User?.FindFirstValue("access_token");
                
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var url = $"{baseUrl}users/{id}";
                Console.WriteLine($"Calling DELETE API: {url}");
                Console.WriteLine($"Authorization: Bearer {token?.Substring(0, Math.Min(20, token.Length))}...");
                
                var response = await _httpClient.DeleteAsync(url);
                
                Console.WriteLine($"API Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("User deleted successfully");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorObj = JsonSerializer.Deserialize<ApiErrorResponse>(errorContent);
                    throw new Exception(errorObj?.ErrorMessage ?? errorObj?.Message ?? $"Failed to delete user. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception deleting user {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<ApiResponse<UserDTO>> UpdateUserAsync(Guid id, UpdateUserDTO dto)
        {
            try
            {
                var baseUrl = _configuration["ApiSettings:BaseUrl"];
                var token = _httpContextAccessor.HttpContext?.User?.FindFirstValue("access_token");
                
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                // Ensure the ID in the DTO matches the URL parameter
                dto.Id = id;
                
                var url = $"{baseUrl}users/{id}";
                Console.WriteLine($"Calling PUT API: {url}");
                Console.WriteLine($"Authorization: Bearer {token?.Substring(0, Math.Min(20, token.Length))}...");
                
                var response = await _httpClient.PutAsJsonAsync(url, dto);
                
                Console.WriteLine($"API Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDTO>>();
                    Console.WriteLine("User updated successfully");
                    return apiResponse!;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorObj = JsonSerializer.Deserialize<ApiErrorResponse>(errorContent);
                    Console.WriteLine($"Failed to update user {id}. Status: {response.StatusCode}, Error: {errorContent}");
                    throw new Exception(errorObj?.ErrorMessage ?? errorObj?.Message ?? $"Failed to update user. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception updating user {id}: {ex.Message}");
                throw;
            }
        }
    }
}
