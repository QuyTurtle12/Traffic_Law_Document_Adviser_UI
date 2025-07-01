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

        [BindProperty]
        public Guid Id { get; set; }

        [BindProperty]
        public string? Name { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
                return NotFound();

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenStore.Token);

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
            if (id == null)
                return NotFound();

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenStore.Token);

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