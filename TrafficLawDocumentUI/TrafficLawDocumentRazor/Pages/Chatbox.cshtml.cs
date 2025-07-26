using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
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

            UserId = Guid.Parse(User.FindFirstValue("sub"));

            await LoadChatHistoryAsync();
        }

        private async Task LoadChatHistoryAsync()
        {
            try
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("API");

                var response = await httpClient.GetAsync($"chathistory/search/{UserId}");
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<GetChatHistoryDto>>>();

                if (result?.Data != null)
                {
                    ChatHistory = result.Data
                        .SelectMany(item => new[]
                        {
                            new ChatMessage { UserId = item.UserId, ChatHistoryId = item.Id, IsUser = false, Text = item.Answer},
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
            public Guid? UserId { get; set; }
            public Guid? ChatHistoryId { get; set; }
            public bool IsUser { get; set; }
            public string Text { get; set; }
        }
    }
}