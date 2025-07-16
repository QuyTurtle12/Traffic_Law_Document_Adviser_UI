using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrafficLawDocumentRazor.Services;
using Util;
using Util.DTOs.ApiResponse;
using Util.DTOs.DocumentCategoryDTOs;
using Util.Paginated;

namespace TrafficLawDocumentRazor.Pages.DocumentCategoryPage
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public string currentUserRole { get; set; } = default!;

        [BindProperty]
        public Guid Id { get; set; }

        [BindProperty]
        public string? Name { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (currentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }

            if (id == null)
                return NotFound();

            // Fetch the category to delete
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentCategoryDTO>>>
                ($"document-categories?pageIndex=1&pageSize=1&idSearch={id}");

            if (response?.Data?.Items == null || !response.Data.Items.Any())
                return NotFound();

            var category = response.Data.Items.First();
            Id = category.Id;
            Name = category.Name;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (currentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }

            if (id == null)
                return NotFound();

            var response = await _httpClient.DeleteAsync($"document-categories/soft-delete/{id}");
            if (!response.IsSuccessStatusCode)
            {
                await ApiErrorHandler.HandleErrorResponse(this, response, "Failed to delete category.");

                return RedirectToPage("./Index");
            }

            TempData["ToastMessage"] = "Category deleted successfully!";
            TempData["ToastType"] = "success";

            return RedirectToPage("./Index");
        }
    }
}