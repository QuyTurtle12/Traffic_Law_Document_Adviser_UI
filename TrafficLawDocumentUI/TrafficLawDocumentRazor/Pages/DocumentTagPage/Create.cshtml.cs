using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrafficLawDocumentRazor.Services;
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

        public string currentUserRole { get; set; } = default!;

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public Guid? ParentTagId { get; set; }

        public List<SelectListItem> ParentTags { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (currentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }

            // Fetch parent tags for dropdown
            var response = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentTagDTO>>>("document-tags?pageIndex=1&pageSize=100");

            if (response?.Data?.Items != null)
            {
                ParentTags = response.Data.Items
                    .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name })
                    .ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (currentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var newTag = new
            {
                name = Name,
                parentTagId = ParentTagId
            };

            var response = await _httpClient.PostAsJsonAsync("document-tags", newTag);
            if (!response.IsSuccessStatusCode)
            {
                await ApiErrorHandler.HandleErrorResponse(this, response, "Failed to create tag.");

                await OnGetAsync();
                return Page();
            }

            TempData["ToastMessage"] = "Tag created successfully!";
            TempData["ToastType"] = "success";

            return RedirectToPage("./Index");
        }
    }
}