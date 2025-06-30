using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Util.DTOs.ApiResponse;
using Util.DTOs.ChatHistoryDTOs;
using Util.DTOs.FeedbackDTOs;

namespace TrafficLawDocumentRazor.Pages.FeedbackPage
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DeleteModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public GetFeedbackDto Feedback { get; set; } = new();

        public async Task OnGetAsync(Guid id)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("API");

            var response = await httpClient.GetAsync($"/api/feedback/{id}");
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<GetFeedbackDto>>();
            if (result.Data != null)
            {
                Feedback = result.Data;
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            HttpClient httpClient = _httpClientFactory.CreateClient("API");
            var response = await httpClient.DeleteAsync($"/api/feedback/{Feedback.Id}");
            if (!response.IsSuccessStatusCode)
            {
                return Page();
            }
            return RedirectToPage("/FeedbackPage/Index");
        }
    }
}
