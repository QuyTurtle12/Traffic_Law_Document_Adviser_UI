using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public string currentUserRole { get; set; } = "";

        [BindProperty]
        public AddLawDocumentDTO LawDocument { get; set; } = new();

        [BindProperty]
        public IFormFile? UploadFile { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> Tags { get; set; } = new();

        [BindProperty]
        public List<Guid> SelectedTagIds { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? "";

            if (currentUserRole != "Staff")
                return RedirectToPage("/Index");

            // Fetch categories
            var catResponse = await _httpClient
                .GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentCategoryDTO>>>(
                    "document-categories?pageIndex=1&pageSize=100");
            if (catResponse?.Data?.Items != null)
            {
                Categories = catResponse.Data.Items
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList();
            }

            // Fetch tags
            var tagResponse = await _httpClient
                .GetFromJsonAsync<ApiResponse<PaginatedList<GetDocumentTagDTO>>>(
                    "document-tags?pageIndex=1&pageSize=100");
            if (tagResponse?.Data?.Items != null)
            {
                Tags = tagResponse.Data.Items
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = t.Name
                    })
                    .ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? "";
            if (currentUserRole != "Staff")
                return RedirectToPage("/Index");

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            // Map selected tag IDs into the DTO
            LawDocument.TagList = SelectedTagIds
                .Select(id => new AddDocumentTagMapDTO { DocumentTagId = id })
                .ToList();  // now TagList is an IEnumerable but materialized

            // Build multipart/form-data
            using var content = new MultipartFormDataContent();

            // Text fields
            content.Add(new StringContent(LawDocument.Title ?? ""), "Title");
            content.Add(new StringContent(LawDocument.DocumentCode ?? ""), "DocumentCode");
            if (LawDocument.CategoryId.HasValue)
            {
                content.Add(new StringContent(
                                LawDocument.CategoryId.Value.ToString()),
                            "CategoryId");
            }
            content.Add(new StringContent(
                            LawDocument.ExpertVerification.ToString()),
                        "ExpertVerification");

            // Materialize TagList into a List so we can use [i]
            var tagList = LawDocument.TagList!.ToList();
            for (int i = 0; i < tagList.Count; i++)
            {
                content.Add(new StringContent(
                                tagList[i].DocumentTagId.ToString()),
                            $"TagList[{i}].DocumentTagId");
            }

            // File upload
            if (UploadFile != null && UploadFile.Length > 0)
            {
                var streamContent = new StreamContent(UploadFile.OpenReadStream());
                streamContent.Headers.ContentType =
                    MediaTypeHeaderValue.Parse(UploadFile.ContentType);
                content.Add(streamContent, "File", UploadFile.FileName);
            }

            // POST to the new upload endpoint
            var response = await _httpClient.PostAsync("law-documents/upload", content);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Failed to upload and create document.");
                await OnGetAsync();
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
