using System.Security.Claims;
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
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public EditModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public string currentUserRole { get; set; } = default!;

        [BindProperty]
        public UpdateLawDocumentDTO LawDocument { get; set; } = new();

        [BindProperty]
        public IFormFile? UploadFile { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> Tags { get; set; } = new();

        [BindProperty]
        public List<Guid> SelectedTagIds { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (currentUserRole != "Staff" && currentUserRole != "Expert")
            {
                return RedirectToPage("/Index");
            }

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

            // Fetch the document to edit
            var docResponse = await _httpClient.GetFromJsonAsync<ApiResponse<PaginatedList<GetLawDocumentDTO>>>($"law-documents?pageIndex=1&pageSize=1&idSearch={id}");
            if (docResponse?.Data == null)
            {
                return NotFound();
            }

            // Get the first document from the response
            var doc = docResponse.Data.Items.First();

            // Map GetLawDocumentDTO to UpdateLawDocumentDTO for editing
            LawDocument = new UpdateLawDocumentDTO
            {
                Title = doc.Title,
                DocumentCode = doc.DocumentCode,
                CategoryId = doc.CategoryId,
                FilePath = doc.FilePath,
                LinkPath = doc.LinkPath,
                ExpertVerification = doc.ExpertVerification,
                TagList = doc.TagList?.Select(t => new AddDocumentTagMapDTO { DocumentTagId = t.Id }).ToList()
            };

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

            LawDocument.TagList = SelectedTagIds.Select(id => new AddDocumentTagMapDTO { DocumentTagId = id }).ToList();

            // Update the document
            var response = await _httpClient.PutAsJsonAsync($"law-documents/{id}", LawDocument);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to update document.");
                await OnGetAsync(id);
                return Page();
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostVerifyAsync(Guid id)
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (currentUserRole != "Expert")
            {
                return RedirectToPage("/Index");
            }

            if (id == Guid.Empty)
            {
                TempData["ToastMessage"] = "Missing document ID";
                TempData["ToastType"] = "error";
                return RedirectToPage("./Index");
            }

            var response = await _httpClient.PostAsync($"law-documents/verification/{id}", new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to verify document.");
                await OnGetAsync(id);
                return Page();
            }

            TempData["ToastMessage"] = "Document verified successfully";
            TempData["ToastType"] = "success";
            return RedirectToPage("./Index");
        }
    }
}
