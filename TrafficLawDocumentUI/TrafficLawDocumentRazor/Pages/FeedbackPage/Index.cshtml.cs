using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Util.Constants;
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
        public string UserRole { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            UserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (string.IsNullOrEmpty(UserRole) || (UserRole != RoleConstants.User && UserRole != RoleConstants.Staff))
            {
                return RedirectToPage("/Index");
            }

            if (UserRole == RoleConstants.User)
            {
                UserId = Guid.Parse(User.FindFirstValue("sub"));

                HttpClient httpClient = _httpClientFactory.CreateClient("API");

                var response = await httpClient.GetAsync($"feedback/user/{UserId}");
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<GetFeedbackDto>>>();
                if (result.Data != null)
                {
                    Feedbacks = result.Data;
                }
            }
            else if (UserRole == RoleConstants.Staff)
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("API");

                var response = await httpClient.GetAsync($"feedback/all");
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<GetFeedbackDto>>>();
                if (result.Data != null)
                {
                    Feedbacks = result.Data;
                }
            }

            return Page();
        }
    }
}
