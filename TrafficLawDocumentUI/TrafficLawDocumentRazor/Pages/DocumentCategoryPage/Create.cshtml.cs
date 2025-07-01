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

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(Name))
            {
                ModelState.AddModelError("Name", "Name is required.");
                return Page();
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenStore.Token);

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