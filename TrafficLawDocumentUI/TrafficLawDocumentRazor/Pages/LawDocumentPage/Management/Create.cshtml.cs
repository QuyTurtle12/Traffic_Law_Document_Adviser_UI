using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Util;
using Util.DTOs.ApiResponse;
using Util.DTOs.DocumentCategoryDTOs;
using Util.DTOs.DocumentTagDTOs;
using Util.DTOs.DocumentTagMapDTOs;
using Util.DTOs.LawDocumentDTOs;
using Util.Paginated;

namespace TrafficLawDocumentRazor.Pages.LawDocumentPage.Management
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task OnGetAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenStore.Token);
            
            // Fetch categories
            var catResponse = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentCategoryDTO>>>(
                "document-categories?pageIndex=1&pageSize=100");
            if (catResponse?.Data?.Items != null)
                Categories = catResponse.Data.Items
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();

            // Fetch tags
            var tagResponse = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentTagDTO>>>("document-tags?pageIndex=1&pageSize=100");
            if (tagResponse?.Data != null)
                Tags = tagResponse.Data.Items
                    .Select(t => new SelectListItem
                    {
                            Value = t.Id.ToString(),
                            Text = t.Name
                    })
                    .ToList();
        }

        [BindProperty]
        public AddLawDocumentDTO LawDocument { get; set; } = new();

        [BindProperty]
        public IFormFile? UploadFile { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> Tags { get; set; } = new();

        [BindProperty]
        public List<Guid> SelectedTagIds { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JwtTokenStore.Token);

            // Map selected tags to DTO
            LawDocument.TagList = SelectedTagIds.Select(id => new AddDocumentTagMapDTO { DocumentTagId = id }).ToList();

            var response = await _httpClient.PostAsJsonAsync("law-documents", LawDocument);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to create document.");
                await OnGetAsync();
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
