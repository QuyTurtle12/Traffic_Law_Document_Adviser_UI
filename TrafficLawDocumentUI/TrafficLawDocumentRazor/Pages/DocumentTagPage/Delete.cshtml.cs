using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Util;
using Util.DTOs.ApiResponse;
using Util.DTOs.DocumentTagDTOs;
using Util.Paginated;

namespace TrafficLawDocumentRazor.Pages.DocumentTagPage
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

            // Fetch the tag to delete
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentTagDTO>>>
                ($"document-tags?pageIndex=1&pageSize=1&idSearch={id}");

            if (response?.Data?.Items == null || !response.Data.Items.Any())
                return NotFound();

            var tag = response.Data.Items.First();
            Id = tag.Id;
            Name = tag.Name;

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

            var response = await _httpClient.DeleteAsync($"document-tags/{id}");
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to delete tag.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}