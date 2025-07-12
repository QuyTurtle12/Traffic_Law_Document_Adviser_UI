using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObject;
using Util.DTOs.UserDTOs;
using TrafficLawDocumentRazor.Services;

namespace TrafficLawDocumentRazor.Pages.UserPage
{
    public class DeleteModel : PageModel
    {
        private readonly IUserApiService _userApiService;

        public DeleteModel(IUserApiService userApiService)
        {
            _userApiService = userApiService;
        }

        public string CurrentUserRole { get; set; } = default!;

        [BindProperty]
        public UserDTO User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            CurrentUserRole = base.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (CurrentUserRole != "Admin")
            {
                return RedirectToPage("/Index");
            }

            var user = await _userApiService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                User = user;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            CurrentUserRole = base.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

            if (CurrentUserRole != "Admin")
            {
                return RedirectToPage("/Index");
            }

            var success = await _userApiService.DeleteUserAsync(id);
            if (!success)
            {
                ModelState.AddModelError("", "Failed to delete user.");
                // Reload the user data for the page
                var user = await _userApiService.GetUserByIdAsync(id);
                if (user != null)
                {
                    User = user;
                }
                return Page();
            }

            return RedirectToPage("./Index");
        }
    }
}
