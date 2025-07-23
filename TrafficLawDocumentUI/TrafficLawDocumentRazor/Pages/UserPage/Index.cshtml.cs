using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObject;
using Util.Paginated;
using Util.DTOs.NewsDTOs;
using Util.DTOs.UserDTOs;
using TrafficLawDocumentRazor.Services;
using System.Text.Json;

namespace TrafficLawDocumentRazor.Pages.UserPage
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IUserApiService _userApiService;

        public IndexModel(ILogger<IndexModel> logger, IUserApiService userApiService)
        {
            _logger = logger;
            _userApiService = userApiService;
        }

        public PaginatedList<UserDTO> UserList { get; set; } = new PaginatedList<UserDTO> { Items = new List<UserDTO>() };

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        [BindProperty]
        public ToggleActiveRequest ToggleRequest { get; set; } = new ToggleActiveRequest();

        public async Task OnGetAsync()
        {
            try
            {
                UserList = await _userApiService.GetUsersAsync(PageIndex, PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users data");
                UserList = new PaginatedList<UserDTO> { Items = new List<UserDTO>() };
                TempData["ErrorMessage"] = ex.Message;
            }
        }

        public async Task<IActionResult> OnPostToggleActiveAsync()
        {
            if (ToggleRequest == null || ToggleRequest.Id == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Invalid request.";
                return RedirectToPage(new { PageIndex, PageSize });
            }
            var user = await _userApiService.GetUserByIdAsync(ToggleRequest.Id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToPage(new { PageIndex, PageSize });
            }
            var updateDto = new UpdateUserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = ToggleRequest.IsActive
            };
            var result = await _userApiService.UpdateUserAsync(user.Id, updateDto);
            if (result != null && result.StatusCode == 200)
            {
                if (ToggleRequest.IsActive == true)
                {
                    TempData["SuccessMessage"] = "User status has been enable.";
                } else
                {
                    TempData["SuccessMessage"] = "User status has been disable.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = result?.Message ?? "Failed to disable user.";
            }
            return RedirectToPage(new { PageIndex, PageSize });
        }
        public class ToggleActiveRequest
        {
            public Guid Id { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
