using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObject;

namespace TrafficLawDocumentRazor.Pages.LawDocumentPage
{
    public class DetailsModel : PageModel
    {
        private readonly TrafficLawDocumentDbContext _context;
        private readonly IConfiguration _configuration;

        public DetailsModel(
            TrafficLawDocumentDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        public LawDocument LawDocument { get; set; } = default!;
        public string ApiBaseUrl { get; private set; } = "";

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null) return NotFound();

            LawDocument = await _context.LawDocuments
                .Include(ld => ld.Category)
                .FirstOrDefaultAsync(m => m.Id == id)
              ?? throw new InvalidOperationException("Document not found");

            ApiBaseUrl = (_configuration["ApiSettings:BaseUrl"] ?? "")
                         .TrimEnd('/');

            return Page();
        }
    }
}
