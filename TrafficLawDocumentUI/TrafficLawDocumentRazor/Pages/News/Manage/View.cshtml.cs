using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BussinessObject;
using Microsoft.EntityFrameworkCore;
using Util.DTOs.NewsDTOs;
using Microsoft.AspNetCore.Authorization;

namespace TrafficLawDocumentRazor.Pages.News.Manage
{
    [AllowAnonymous]
    public class ViewModel : PageModel
    {
        private readonly TrafficLawDocumentDbContext _context;

        public ViewModel(TrafficLawDocumentDbContext context)
        {
            _context = context;
        }

        public GetNewsDTO? News { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            News = await _context.News
                .Where(n => n.Id == id && n.DeletedTime == null)
                .Select(n => new GetNewsDTO
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    PublishedDate = n.PublishedDate,
                    Author = n.Author,
                    ImageUrl = n.ImageUrl,
                    EmbeddedUrl = n.EmbeddedUrl,
                    CreatedTime = n.CreatedTime,
                    LastUpdatedTime = n.LastUpdatedTime,
                    UserId = n.UserId
                })
                .FirstOrDefaultAsync();

            if (News == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}