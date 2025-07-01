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
        private readonly BussinessObject.TrafficLawDocumentDbContext _context;

        public DetailsModel(BussinessObject.TrafficLawDocumentDbContext context)
        {
            _context = context;
        }

        public LawDocument LawDocument { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lawdocument = await _context.LawDocuments.Include(ld => ld.Category).FirstOrDefaultAsync(m => m.Id == id);
            if (lawdocument == null)
            {
                return NotFound();
            }
            else
            {
                LawDocument = lawdocument;
                // Now it's safe to access LawDocument.Category
                if (LawDocument.Category != null)
                {
                    LawDocument.Category.Name = lawdocument.Category.Name;
                }
            }
            return Page();
        }
    }
}
