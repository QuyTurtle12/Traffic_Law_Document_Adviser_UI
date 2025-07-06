using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Util;

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

            var payload = new { name = Name };
            var response = await _httpClient.PostAsJsonAsync("document-categories", payload);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to create category.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}