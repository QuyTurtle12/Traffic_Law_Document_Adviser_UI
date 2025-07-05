using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Util;
using Util.DTOs.ApiResponse;
using Util.DTOs.AuthDTOs;

namespace TrafficLawDocumentRazor.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _http;
        public LoginModel(IHttpClientFactory httpFactory)
            => _http = httpFactory.CreateClient("API");

        [BindProperty]
        public LoginDTO Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(string returnUrl = "/")
        {
            if (!ModelState.IsValid)
                return Page();

            // 1) Send credentials to API
            var resp = await _http.PostAsJsonAsync("auth/login", Input);

            if (!resp.IsSuccessStatusCode)
            {
                // parse { errorCode, errorMessage }
                var err = await resp.Content.ReadFromJsonAsync<ErrorResponseDTO>();
                TempData["ToastMessage"] = err?.ErrorMessage ?? "Login failed";
                TempData["ToastType"] = "error";
                return Page();
            }

            // 2) parse success envelope
            var apiResult = await resp.Content.ReadFromJsonAsync<ApiResponse<AuthResponseDTO>>();
            if (apiResult?.Code != "SUCCESS")
            {
                TempData["ToastMessage"] = apiResult?.Message ?? "Login failed";
                TempData["ToastType"] = "error";
                return Page();
            }

            // 3) build a cookie principal from the JWT
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(apiResult.Data.Token);

            // Create list of claims from JWT and add the token itself
            var claims = jwtToken.Claims.ToList();
            claims.Add(new Claim("access_token", apiResult.Data.Token));

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme,
                "email",              // NameClaimType
                ClaimTypes.Role       // RoleClaimType
            );
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            TempData["ToastMessage"] = "Login successful!";
            TempData["ToastType"] = "success";
            return LocalRedirect(returnUrl);
        }

        // for deserializing { errorCode, errorMessage }
        private class ErrorResponseDTO
        {
            [JsonPropertyName("errorCode")]
            public string ErrorCode { get; set; } = "";
            [JsonPropertyName("errorMessage")]
            public string ErrorMessage { get; set; } = "";
        }
    }
}
