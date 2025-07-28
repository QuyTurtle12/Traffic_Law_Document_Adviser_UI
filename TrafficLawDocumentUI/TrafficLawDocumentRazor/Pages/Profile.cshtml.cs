using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace TrafficLawDocumentRazor
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        public string? FullName { get; private set; }
        public string? Email { get; private set; }
        public IEnumerable<string> Roles { get; private set; }

        public void OnGet()
        {
            // Adjust claim types as appropriate for your authentication system
            FullName = User.FindFirst("name")?.Value
                    ?? User.FindFirst("FullName")?.Value // for custom claim types
                    ?? User.Identity?.Name; // fallback

            Email = User.FindFirst(ClaimTypes.Email)?.Value
                 ?? User.FindFirst("email")?.Value
                 ?? "";

            Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
        }
    }
}
