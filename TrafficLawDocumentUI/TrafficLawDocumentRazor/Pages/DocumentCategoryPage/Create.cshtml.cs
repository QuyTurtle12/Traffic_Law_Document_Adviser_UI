using System.Security.Claims;
using System.Text.Json;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrafficLawDocumentRazor.Services;
using Util;
using Util.DTOs.ApiResponse;
using Util.DTOs.DocumentTagDTOs;
using Util.Paginated;

namespace TrafficLawDocumentRazor.Pages.DocumentCategoryPage
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public string currentUserRole { get; set; } = default!;

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (currentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (currentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Name))
            {
                ModelState.AddModelError("Name", "Name is required.");
                return Page();
            }

            var newCategoryData = new { name = Name };
            var response = await _httpClient.PostAsJsonAsync("document-categories", newCategoryData);

            if (!response.IsSuccessStatusCode)
            {
                await ApiErrorHandler.HandleErrorResponse(this, response, "Failed to create category.");

                var content = await response.Content.ReadAsStringAsync();
                //TempData["ToastMessage"] = $"Error: {content}";
                TempData["ToastType"] = "error";
                using var doc = JsonDocument.Parse(content);
                string message = doc.RootElement.GetProperty("message").GetString();

                TempData["ToastMessage"] = $"Error: {message}";
                TempData["ToastType"] = "error";

                return Page();
            }

            TempData["ToastMessage"] = "Category created successfully!";
            TempData["ToastType"] = "success";

            return RedirectToPage("./Index");
        }
    }
}