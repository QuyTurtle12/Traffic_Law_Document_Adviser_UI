using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TrafficLawDocumentRazor.Pages
{
    public class LogoutModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public bool Confirm { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!Confirm)
            {
                // Show the confirmation page
                return Page();
            }

            // User confirmed logout
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            TempData["ToastMessage"] = "You have been successfully logged out. Thank you for using the Traffic Law Document Management System.";
            TempData["ToastType"] = "success";
            
            return RedirectToPage("/Login");
        }
    }
}
