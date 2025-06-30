using Microsoft.AspNetCore.Mvc.RazorPages;
using Util.DTOs.ApiResponse;
using Util.DTOs.FeedbackDTOs;

namespace TrafficLawDocumentRazor.Pages.FeedbackPage
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IList<GetFeedbackDto> Feedbacks { get;set; } = default!;
        public Guid UserId { get; set; }
        public async Task OnGetAsync()
        {
            UserId = Guid.Parse("82BAC5B2-E3D3-40CF-82B6-6EFDA10B2EDB");
            HttpClient httpClient = _httpClientFactory.CreateClient("API");

            var response = await httpClient.GetAsync($"/api/feedback/user/{UserId}");
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<GetFeedbackDto>>>();
            if(result.Data != null)
            {
                Feedbacks = result.Data;
            }
        }
    }
}
