using System.Security.Claims;
using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

            var payload = new { id = Id, name = Name };
            var response = await _httpClient.PutAsJsonAsync($"document-categories/{Id}", payload);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to update category.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }

}