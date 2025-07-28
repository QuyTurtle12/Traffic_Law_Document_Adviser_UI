using BCrypt.Net;
using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TrafficLawDocumentRazor.Services;
using Util.DTOs.UserDTOs;

namespace TrafficLawDocumentRazor.Pages.UserPage
{
    public class CreateModel : PageModel
    {
        private readonly IUserApiService _userApiService;
        public CreateModel(IUserApiService userApiService)
        {
            _userApiService = userApiService;
        }
        [BindProperty]
        public CreateUserDTO NewUser { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public string CurrentUserRole { get; set; } = default!;
        public IActionResult OnGet()
        {
            CurrentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            if (CurrentUserRole != "Admin")
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var result = await _userApiService.CreateUserAsync(NewUser);
                Console.WriteLine($"Razor Page - Result: StatusCode={result?.StatusCode}, Message={result?.Message}");
                
                if (result != null && result.StatusCode == 201)
                {
                    Console.WriteLine("Razor Page - Success: Redirecting to Index");
                    TempData["SuccessMessage"] = result.Message ?? "User created successfully.";
                    return RedirectToPage("./Index");
                }
                Console.WriteLine($"Razor Page - Error: StatusCode={result?.StatusCode}, Message={result?.Message}");
                ErrorMessage = result?.Message ?? "Failed to create user.";
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Razor Page - Exception: {ex.Message}");
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
