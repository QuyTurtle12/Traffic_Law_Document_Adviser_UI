using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Util;
using Util.DTOs.ApiResponse;
using Util.DTOs.DocumentTagDTOs;
using Util.Paginated; // For JwtTokenStore if needed

namespace TrafficLawDocumentRazor.Pages.DocumentTagPage
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

        [BindProperty]
        public Guid? ParentTagId { get; set; }

        public List<SelectListItem> ParentTags { get; set; } = new();

        public async Task OnGetAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenStore.Token);

            // Fetch parent tags for dropdown
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentTagDTO>>>("document-tags?pageIndex=1&pageSize=100");
            if (response?.Data?.Items != null)
            {
                ParentTags = response.Data.Items
                    .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name })
                    .ToList();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenStore.Token);

            var newTag = new
            {
                name = Name,
                parentTagId = ParentTagId
            };

            var response = await _httpClient.PostAsJsonAsync("document-tags", newTag);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to create tag.");
                await OnGetAsync();
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}