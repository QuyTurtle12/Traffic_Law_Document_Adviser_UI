using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using TrafficLawDocumentRazor.Services;
using Util.DTOs.NewsDTOs;
using Util.DTOs.UserDTOs;
using Util.Paginated;

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

        // Search and Filter Properties
        [BindProperty(SupportsGet = true)]
        public string? NameSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? EmailSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? RoleFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty]
        public ToggleActiveRequest ToggleRequest { get; set; } = new ToggleActiveRequest();

        public string CurrentUserRole { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                CurrentUserRole = base.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
                if (CurrentUserRole != "Admin")
                {
                    return RedirectToPage("/Index");
                }
                UserList = await _userApiService.GetUsersAsync(PageIndex, PageSize);
                
                if (UserList?.Items != null)
                {
                    UserList.Items = UserList.Items.Where(x => x.Role != "Admin").ToList();
                }

                if (UserList?.Items != null)
                {
                    var filteredItems = UserList.Items.AsQueryable();

                    // Search by name
                    if (!string.IsNullOrWhiteSpace(NameSearch))
                    {
                        var nameSearchLower = NameSearch.ToLower();
                        filteredItems = filteredItems.Where(u => 
                            u.FullName.ToLower().Contains(nameSearchLower)
                        );
                    }

                    // Search by email
                    if (!string.IsNullOrWhiteSpace(EmailSearch))
                    {
                        var emailSearchLower = EmailSearch.ToLower();
                        filteredItems = filteredItems.Where(u => 
                            u.Email.ToLower().Contains(emailSearchLower)
                        );
                    }

                    // Apply role filter
                    if (!string.IsNullOrWhiteSpace(RoleFilter) && RoleFilter != "All")
                    {
                        filteredItems = filteredItems.Where(u => u.Role == RoleFilter);
                    }

                    // Apply status filter
                    if (!string.IsNullOrWhiteSpace(StatusFilter) && StatusFilter != "All")
                    {
                        bool isActive = StatusFilter == "Active";
                        filteredItems = filteredItems.Where(u => u.IsActive == isActive);
                    }

                    // Update the items with filtered results
                    UserList.Items = filteredItems.ToList();
                    
                    // Recalculate pagination
                    UserList.TotalCount = UserList.Items.Count;
                    UserList.PageNumber = PageIndex;
                    UserList.PageSize = PageSize;
                    UserList.TotalPages = (int)Math.Ceiling((double)UserList.TotalCount / PageSize);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users data");
                UserList = new PaginatedList<UserDTO> { Items = new List<UserDTO>() };
                TempData["ErrorMessage"] = ex.Message;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostToggleActiveAsync()
        {
            try
            {
                if (ToggleRequest == null || ToggleRequest.Id == Guid.Empty)
                {
                    TempData["ErrorMessage"] = "Invalid request.";
                    return RedirectToPage(new { PageIndex, PageSize, NameSearch, EmailSearch, RoleFilter, StatusFilter });
                }

                var user = await _userApiService.GetUserByIdAsync(ToggleRequest.Id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToPage(new { PageIndex, PageSize, NameSearch, EmailSearch, RoleFilter, StatusFilter });
                }

                // Create update DTO with all required fields
                var updateDto = new UpdateUserDTO
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role,
                    IsActive = ToggleRequest.IsActive
                };

                _logger.LogInformation($"Attempting to update user {user.Id} with IsActive={ToggleRequest.IsActive}");

                var result = await _userApiService.UpdateUserAsync(user.Id, updateDto);
                
                if (result != null && result.StatusCode == 200)
                {
                    if (ToggleRequest.IsActive == true)
                    {
                        TempData["SuccessMessage"] = "User status has been enabled.";
                    } 
                    else
                    {
                        TempData["SuccessMessage"] = "User status has been disabled.";
                    }
                }
                else
                {
                    var errorMessage = result?.Message ?? "Failed to update user status.";
                    _logger.LogError($"Failed to update user {user.Id}: {errorMessage}");
                    TempData["ErrorMessage"] = errorMessage;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while toggling user active status");
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToPage(new { PageIndex, PageSize, NameSearch, EmailSearch, RoleFilter, StatusFilter });
        }
        public class ToggleActiveRequest
        {
            public Guid Id { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
