using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using TrafficLawDocumentRazor.Hubs;
using Util.DTOs.ApiResponse;
using Util.DTOs.ChatHistoryDTOs;

namespace TrafficLawDocumentRazor.Pages
{
    public class ChatboxModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatboxModel(IHttpClientFactory httpClientFactory, IHubContext<ChatHub> hubContext)
        {
            _httpClientFactory = httpClientFactory;
            _hubContext = hubContext;
        }

        [BindProperty]
        public string UserMessage { get; set; }
        public string ApiResponse { get; set; }
        public Guid UserId { get; set; }
        public bool IsLoading { get; set; }
        public List<ChatMessage> ChatHistory { get; set; } = new List<ChatMessage>();

        public async Task OnGetAsync()
        {
            // Initialize page
            ApiResponse = string.Empty;
            IsLoading = false;
            //Temporarily hardcoding the UserId
            UserId = Guid.Parse("82BAC5B2-E3D3-40CF-82B6-6EFDA10B2EDB");

            await LoadChatHistoryAsync();
        }

        private async Task LoadChatHistoryAsync()
        {
            try
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("API");

                var response = await httpClient.GetAsync($"/api/chathistory/search/{UserId}");
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<GetChatHistoryDto>>>();

                if (result?.Data != null)
                {
                    ChatHistory = result.Data
                        .SelectMany(item => new[]
                        {
                            new ChatMessage { IsUser = false, Text = item.Answer },
                            new ChatMessage { IsUser = true, Text = item.Question }
                            
                        })
                        .Reverse()
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public class ChatMessage
        {
            public bool IsUser { get; set; }
            public string Text { get; set; }
        }
    }
}