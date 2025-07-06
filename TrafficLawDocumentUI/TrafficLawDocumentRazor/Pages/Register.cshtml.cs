using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Util;
using Util.DTOs.ApiResponse;
using Util.DTOs.AuthDTOs;

namespace TrafficLawDocumentRazor.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient _http;
        public RegisterModel(IHttpClientFactory httpFactory)
            => _http = httpFactory.CreateClient("API");

        [BindProperty]
        public RegisterUserDTO Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var resp = await _http.PostAsJsonAsync("auth/register", Input);
            var raw = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                TempData["ToastMessage"] = $"Registration failed: {raw}. Please check your information and try again.";
                TempData["ToastType"] = "error";
                return Page();
            }

            var apiResult = await resp.Content.ReadFromJsonAsync<ApiResponse<object>>();
            if (apiResult?.Code != "SUCCESS")
            {
                TempData["ToastMessage"] = apiResult?.Message ?? "Registration failed. Please verify your information and try again.";
                TempData["ToastType"] = "error";
                return Page();
            }

            TempData["ToastMessage"] = "Account created successfully! You can now log in with your new credentials.";
            TempData["ToastType"] = "success";
            return RedirectToPage("/Login");
        }
    }
}
