using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrafficLawDocumentRazor.Services;
using Util;
using Util.DTOs.ApiResponse;
using Util.DTOs.DocumentTagDTOs;
using Util.Paginated;

namespace TrafficLawDocumentRazor.Pages.DocumentTagPage
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
        public Guid? ParentTagId { get; set; }

        public List<SelectListItem> ParentTags { get; set; } = new();

        [TempData]
        public string? ErrorMessage { get; set; }

        private class ErrorResponse
        {
            public string ErrorCode { get; set; } = string.Empty;
            public string ErrorMessage { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (currentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }

            if (id == null)
                return NotFound();

            // Fetch all tags for parent tag dropdown
            var tagResponse = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentTagDTO>>>("document-tags?pageIndex=1&pageSize=100");
            if (tagResponse?.Data?.Items != null)
            {
                ParentTags = tagResponse.Data.Items
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Name
                    })
                    .ToList();
            }

            // Fetch the current tag to edit
            var currentTag = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentTagDTO>>>($"document-tags?pageIndex=1&pageSize=1&idSearch={id}");
            if (currentTag?.Data?.Items == null || !currentTag.Data.Items.Any())
                return NotFound();

            var tag = currentTag.Data.Items.First();
            Name = tag.Name;
            ParentTagId = tag.ParentTagId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (currentUserRole != "Staff")
            {
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync(id);
                return Page();
            }

            var updateTag = new
            {
                name = Name,
                parentTagId = ParentTagId
            };

            var response = await _httpClient.PutAsJsonAsync($"document-tags/{id}", updateTag);

            if (!response.IsSuccessStatusCode)
            {
                await ApiErrorHandler.HandleErrorResponse(this, response, "Failed to edit tag.");

                await OnGetAsync(id);
                return Page();
            }

            TempData["ToastMessage"] = "Tag updated successfully!";
            TempData["ToastType"] = "success";

            return RedirectToPage("./Index");
        }
    }
}