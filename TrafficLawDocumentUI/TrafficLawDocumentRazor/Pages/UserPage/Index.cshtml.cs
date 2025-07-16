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
    }
}
