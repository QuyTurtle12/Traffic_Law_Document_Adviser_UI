using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Util.DTOs.ApiResponse;
using Util.DTOs.ChatHistoryDTOs;
using Util.DTOs.FeedbackDTOs;

namespace TrafficLawDocumentRazor.Pages.FeedbackPage
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public PostFeedbackDto Feedback { get; set; } = new();

        public async Task OnGetAsync(Guid chatHistoryId, Guid userId)
        {
            
            HttpClient httpClient = _httpClientFactory.CreateClient("API");

            var response = await httpClient.GetAsync($"/api/chathistory/{chatHistoryId}");
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<GetChatHistoryDto>>();
            if(result?.Data != null)
            {
                Feedback.AIAnswer = result.Data.Answer;
            }

            Feedback.UserId = userId;
            Feedback.ChatHistory = chatHistoryId;
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            HttpClient httpClient = _httpClientFactory.CreateClient("API");
            var response = await httpClient.PostAsJsonAsync($"/api/feedback/", Feedback);
            if (!response.IsSuccessStatusCode)
            {
                return Page();
            }
            return RedirectToPage("/FeedbackPage/Index");
        }
    }
}
