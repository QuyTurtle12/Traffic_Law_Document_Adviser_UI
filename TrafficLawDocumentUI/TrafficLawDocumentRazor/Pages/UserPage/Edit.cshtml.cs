// Edit User Page - API Integration
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Util.DTOs.UserDTOs;
using TrafficLawDocumentRazor.Services;

namespace TrafficLawDocumentRazor.Pages.UserPage
{
    public class EditModel : PageModel
    {
        private readonly IUserApiService _userApiService;

        public EditModel(IUserApiService userApiService)
        {
            _userApiService = userApiService;
        }

        public string CurrentUserRole { get; set; } = default!;
        public string CurrentUserId { get; set; } = default!;
        public string? ErrorMessage { get; set; }

        [BindProperty]
        public UpdateUserDTO User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                CurrentUserRole = base.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
                CurrentUserId = base.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

                // Check authorization
                if (string.IsNullOrEmpty(CurrentUserRole))
                {
                    return RedirectToPage("/Login");
                }

                // Admin can edit any user, others can only edit their own profile
                if (CurrentUserRole != "Admin" && CurrentUserId != id.ToString())
                {
                    return RedirectToPage("/Index");
                }

                var user = await _userApiService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Map UserDTO to UpdateUserDTO
                User = new UpdateUserDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    IsActive = user.IsActive
                };

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading user: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            try
            {
                CurrentUserRole = base.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
                CurrentUserId = base.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

                // Check authorization
                if (string.IsNullOrEmpty(CurrentUserRole))
                {
                    return RedirectToPage("/Login");
                }

                // Admin can edit any user, others can only edit their own profile
                if (CurrentUserRole != "Admin" && CurrentUserId != id.ToString())
                {
                    return RedirectToPage("/Index");
                }

                if (!ModelState.IsValid)
                {
                    // Reload user data for the page
                    var user = await _userApiService.GetUserByIdAsync(id);
                    if (user != null)
                    {
                        User = new UpdateUserDTO
                        {
                            Id = user.Id,
                            FullName = user.FullName,
                            Email = user.Email,
                            Role = user.Role,
                            IsActive = user.IsActive
                        };
                    }
                    return Page();
                }

                var result = await _userApiService.UpdateUserAsync(id, User);
                
                if (result != null && result.StatusCode == 200)
                {
                    TempData["SuccessMessage"] = result.Message ?? "User updated successfully.";
                    return RedirectToPage("./Index");
                }
                else
                {
                    ErrorMessage = result?.Message ?? "Failed to update user.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating user: {ex.Message}";
                return Page();
            }
        }
    }
}




