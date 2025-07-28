using System.Security.Claims;
using System.Text.Json;
using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrafficLawDocumentRazor.Services;
using Util;
using Util.DTOs.ApiResponse;
using Util.DTOs.DocumentCategoryDTOs;
using Util.Paginated;

namespace TrafficLawDocumentRazor.Pages.DocumentCategoryPage
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public string currentUserRole { get; set; } = default!;

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public Guid Id { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (currentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }

            if (id == null)
            {
                return NotFound();
            }

            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentCategoryDTO>>>
                ($"document-categories?pageIndex=1&pageSize=1&idSearch={id}");

            var category = response?.Data?.Items?.First();

            if (category == null)
            {
                return NotFound();
            }

            Id = category.Id;
            Name = category.Name;
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

            var updatedCategoryData = new { id = Id, name = Name };
            var response = await _httpClient.PutAsJsonAsync($"document-categories/{Id}", updatedCategoryData);

            if (!response.IsSuccessStatusCode)
            {
                await ApiErrorHandler.HandleErrorResponse(this, response, "Failed to edit category.");

                var content = await response.Content.ReadAsStringAsync();
                //TempData["ToastMessage"] = $"Error: {content}";
                TempData["ToastType"] = "error";
                using var doc = JsonDocument.Parse(content);
                string message = doc.RootElement.GetProperty("message").GetString();

                TempData["ToastMessage"] = $"Error: {message}";
                TempData["ToastType"] = "error";

                await OnGetAsync(Id);
                return Page();
            }

            TempData["ToastMessage"] = "category updated successfully!";
            TempData["ToastType"] = "success";

            return RedirectToPage("./Index");
        }
    }

}