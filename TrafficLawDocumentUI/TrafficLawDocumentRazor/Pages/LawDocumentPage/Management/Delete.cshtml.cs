using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Util;

namespace TrafficLawDocumentRazor.Pages.LawDocumentPage.Management
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
        public string? Title { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (currentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }

            if (id == null)
                return NotFound();

            var response = await _httpClient.GetAsync($"law-documents?pageIndex=1&pageSize=1&idSearch={id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var apiResponse = await response.Content.ReadFromJsonAsync<Util.DTOs.ApiResponse.ApiResponse<Util.Paginated.PaginatedList<Util.DTOs.LawDocumentDTOs.GetLawDocumentDTO>>>();
            var doc = apiResponse?.Data?.Items?.FirstOrDefault();
            if (doc == null)
                return NotFound();

            Id = doc.Id;
            Title = doc.Title;
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

            var response = await _httpClient.DeleteAsync($"law-documents/soft-delete/{id}");
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to delete document.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}