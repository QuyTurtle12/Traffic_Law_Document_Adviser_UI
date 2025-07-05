using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Util.DTOs.ApiResponse;
using Util.DTOs.NewsDTOs;
using Util.Paginated;

namespace TrafficLawDocumentRazor.Services
{
    public interface INewsApiService
    {
        Task<PaginatedList<GetNewsDTO>> GetNewsAsync(int pageIndex = 1, int pageSize = 10, string? titleFilter = null, string? authorFilter = null, string? contentFilter = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<GetNewsDTO?> GetNewsByIdAsync(Guid id);
        Task<GetNewsDTO?> CreateNewsAsync(AddNewsDTO news);
        Task<GetNewsDTO?> UpdateNewsAsync(Guid id, AddNewsDTO news);
        Task<bool> DeleteNewsAsync(Guid id);
        Task<List<GetNewsDTO>?> SyncNewsAsync();
    }

    public class NewsApiService : INewsApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NewsApiService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public NewsApiService(HttpClient httpClient, ILogger<NewsApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<PaginatedList<GetNewsDTO>> GetNewsAsync(int pageIndex = 1, int pageSize = 10, string? titleFilter = null, string? authorFilter = null, string? contentFilter = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"pageIndex={pageIndex}",
                    $"pageSize={pageSize}"
                };

                if (!string.IsNullOrEmpty(titleFilter))
                    queryParams.Add($"titleFilter={Uri.EscapeDataString(titleFilter)}");
                
                if (!string.IsNullOrEmpty(authorFilter))
                    queryParams.Add($"authorFilter={Uri.EscapeDataString(authorFilter)}");
                
                if (!string.IsNullOrEmpty(contentFilter))
                    queryParams.Add($"contentFilter={Uri.EscapeDataString(contentFilter)}");
                
                if (startDate.HasValue)
                    queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                
                if (endDate.HasValue)
                    queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

                var url = $"/api/News?{string.Join("&", queryParams)}";
                _logger.LogInformation("Calling API: {Url}", url);
                
                var response = await _httpClient.GetAsync(url);
                
                _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("API Response Content: {Content}", responseContent);
                    
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PaginatedList<GetNewsDTO>>>(_jsonOptions);
                    return apiResponse?.Data ?? new PaginatedList<GetNewsDTO> { Items = new List<GetNewsDTO>() };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to get news. Status: {StatusCode}, Error: {Error}", response.StatusCode, errorContent);
                    return new PaginatedList<GetNewsDTO> { Items = new List<GetNewsDTO>() };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news");
                return new PaginatedList<GetNewsDTO> { Items = new List<GetNewsDTO>() };
            }
        }

        public async Task<GetNewsDTO?> GetNewsByIdAsync(Guid id)
        {
            try
            {
                var url = $"/api/News/{id}";
                _logger.LogInformation("Calling API: {Url}", url);
                
                var response = await _httpClient.GetAsync(url);
                
                _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("API Response Content: {Content}", responseContent);
                    
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<GetNewsDTO>>(_jsonOptions);
                    return apiResponse?.Data;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to get news by ID {Id}. Status: {StatusCode}, Error: {Error}", id, response.StatusCode, errorContent);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting news by ID {Id}", id);
                return null;
            }
        }

        public async Task<GetNewsDTO?> CreateNewsAsync(AddNewsDTO news)
        {
            try
            {
                var json = JsonSerializer.Serialize(news, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logger.LogInformation("Creating news with data: {Data}", json);
                
                var response = await _httpClient.PostAsync("/api/News", content);
                
                _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("API Response Content: {Content}", responseContent);
                    
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<GetNewsDTO>>(_jsonOptions);
                    return apiResponse?.Data;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to create news. Status: {StatusCode}, Error: {Error}", response.StatusCode, errorContent);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating news");
                return null;
            }
        }

        public async Task<GetNewsDTO?> UpdateNewsAsync(Guid id, AddNewsDTO news)
        {
            try
            {
                var json = JsonSerializer.Serialize(news, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                _logger.LogInformation("Updating news {Id} with data: {Data}", id, json);
                
                var response = await _httpClient.PutAsync($"/api/News/{id}", content);
                
                _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("API Response Content: {Content}", responseContent);
                    
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<GetNewsDTO>>(_jsonOptions);
                    return apiResponse?.Data;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to update news {Id}. Status: {StatusCode}, Error: {Error}", id, response.StatusCode, errorContent);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating news {Id}", id);
                return null;
            }
        }

        public async Task<bool> DeleteNewsAsync(Guid id)
        {
            try
            {
                var url = $"/api/News/{id}";
                _logger.LogInformation("Calling API: {Url}", url);
                
                var response = await _httpClient.DeleteAsync(url);
                
                _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to delete news {Id}. Status: {StatusCode}, Error: {Error}", id, response.StatusCode, errorContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting news {Id}", id);
                return false;
            }
        }

        public async Task<List<GetNewsDTO>?> SyncNewsAsync()
        {
            try
            {
                _logger.LogInformation("Calling API: /api/News/sync");
                
                var response = await _httpClient.PostAsync("/api/News/sync", null);
                
                _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("API Response Content: {Content}", responseContent);
                    
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<GetNewsDTO>>>(_jsonOptions);
                    return apiResponse?.Data;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to sync news. Status: {StatusCode}, Error: {Error}", response.StatusCode, errorContent);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing news");
                return null;
            }
        }
    }
} 