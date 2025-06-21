using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Util; // For JwtTokenStore if needed

namespace TrafficLawDocumentRazor.Pages.LawDocumentPage
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        [BindProperty]
        public Guid Id { get; set; }

        [BindProperty]
        public string? Title { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
                return NotFound();

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenStore.Token);

            var response = await _httpClient.GetAsync($"/api/law-documents?pageIndex=1&pageSize=1&idSearch={id}");
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
            if (id == null)
                return NotFound();

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenStore.Token);

            var response = await _httpClient.DeleteAsync($"/api/law-documents/soft-delete/{id}");
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to delete document.");
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}