using Microsoft.AspNetCore.SignalR;
using Util.DTOs.ApiResponse;
using Util.DTOs.ChatHistoryDTOs;

namespace TrafficLawDocumentRazor.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatHub(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendMessage(string userId, string message, string modelName)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                await Clients.Caller.SendAsync("ShowError", "Message cannot be empty");
                return;
            }

            try
            {
                // Show user message immediately
                await Clients.Group($"User_{userId}").SendAsync("ReceiveMessage", message, true);
                
                // Show loading state
                await Clients.Group($"User_{userId}").SendAsync("SetLoadingState", true);

                // Call your API (hidden from client)
                HttpClient httpClient = _httpClientFactory.CreateClient("API");

                var chatRequest = new PostChatHistoryDto
                {
                    UserId = Guid.Parse(userId),
                    Question = message
                };
                var response = await httpClient.PostAsJsonAsync($"chathistory?modelName={modelName}", chatRequest);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
                    var chatId = responseContent.Result.Data;
                    var response2 = await httpClient.GetAsync($"chathistory/{chatId}");
                    var result = await response2.Content.ReadFromJsonAsync<ApiResponse<GetChatHistoryDto>>();

                    // Send the API response
                    if (result?.Data != null)
                    {
                        await Clients.Group($"User_{userId}").SendAsync("ReceiveMessage", result.Data, false);
                    }
                }
                else
                {
                    await Clients.Group($"User_{userId}").SendAsync("ShowError", $"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                await Clients.Group($"User_{userId}").SendAsync("ShowError", $"Error: {ex.Message}");
            }
            finally
            {
                // Remove loading state
                await Clients.Group($"User_{userId}").SendAsync("SetLoadingState", false);
            }
        }
        public async Task SendMessageToUser(string userId, string message, bool isUser)
        {
            await Clients.Group($"User_{userId}").SendAsync("ReceiveMessage", message, isUser);
        }

        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public async Task LeaveUserGroup(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}