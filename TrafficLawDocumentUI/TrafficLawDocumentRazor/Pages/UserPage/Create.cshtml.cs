using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BussinessObject;
using BCrypt.Net;
using Util.DTOs.UserDTOs;
using TrafficLawDocumentRazor.Services;

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
        public IActionResult OnGet()
        {
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
                if (result != null && result.StatusCode == 201)
                {
                    TempData["SuccessMessage"] = result.Message ?? "User created successfully.";
                    return RedirectToPage("./Index");
                }
                ErrorMessage = result?.Message ?? "Failed to create user.";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
        }
    }
}
