using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace TrafficLawDocumentRazor
{
    [Authorize(Roles = "Admin")]
    public class ProfileModel : PageModel
    {
        public string CurrentUserId { get; private set; }

        public IEnumerable<string> Roles { get; private set; }
        public IEnumerable<Claim> Claims { get; private set; }

        public void OnGet()
        {
            CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                 ?? User.FindFirstValue("sub");

            Roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);

            Claims = User.Claims;
        }
    }

}
