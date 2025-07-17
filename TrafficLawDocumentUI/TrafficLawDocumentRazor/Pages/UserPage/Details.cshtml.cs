using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObject;

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

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
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
