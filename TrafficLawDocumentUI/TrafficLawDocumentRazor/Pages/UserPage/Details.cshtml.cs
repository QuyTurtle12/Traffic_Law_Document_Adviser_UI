using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TrafficLawDocumentRazor.Pages.UserPage
{
    public class DetailsModel : PageModel
    {
        private readonly BussinessObject.TrafficLawDocumentDbContext _context;

        public DetailsModel(BussinessObject.TrafficLawDocumentDbContext context)
        {
            _context = context;
        }

        public User User { get; set; } = default!;
        public string CurrentUserRole { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {

            CurrentUserRole = base.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            if (CurrentUserRole != "Admin")
            {
                return RedirectToPage("/Index");
            }
            if (id == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return NotFound();
            }
            else
            {
                User = user;
            }
            return Page();
        }
    }
}
