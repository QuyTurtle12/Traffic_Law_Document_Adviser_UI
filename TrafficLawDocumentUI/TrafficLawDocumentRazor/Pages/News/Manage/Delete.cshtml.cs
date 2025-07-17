using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using System.Security.Claims;
using Util.DTOs.NewsDTOs;
using Util.DTOs.ApiResponse;

namespace TrafficLawDocumentRazor.Pages.News.Manage
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }
        public string CurrentUserRole { get; set; } = default!;
        [BindProperty]
        public Util.DTOs.NewsDTOs.GetNewsDTO? News { get; set; }
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            CurrentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            if (CurrentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }
            var response = await _httpClient.GetFromJsonAsync<Util.DTOs.ApiResponse.ApiResponse<Util.DTOs.NewsDTOs.GetNewsDTO>>($"news/{id}");
            News = response?.Data;
            if (News == null)
            {
                return NotFound();
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            CurrentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            if (CurrentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }
            try
            {
                var response = await _httpClient.DeleteAsync($"news/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorObj = System.Text.Json.JsonSerializer.Deserialize<Util.DTOs.ApiResponse.ApiErrorResponse>(errorContent);
                    TempData["ErrorMessage"] = errorObj?.ErrorMessage ?? errorObj?.Message ?? "Failed to delete the news article. Please try again.";
                    return RedirectToPage("Index");
                }
                TempData["SuccessMessage"] = "News article deleted successfully!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToPage("Index");
            }
        }
    }
} 