using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObject;

namespace TrafficLawDocumentRazor.Pages.LawDocumentPage.Management
{
    public class DeleteModel : PageModel
    {
        private readonly BussinessObject.TrafficLawDocumentDbContext _context;

        public DeleteModel(BussinessObject.TrafficLawDocumentDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LawDocument LawDocument { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lawdocument = await _context.LawDocuments.FirstOrDefaultAsync(m => m.Id == id);

            if (lawdocument == null)
            {
                return NotFound();
            }
            else
            {
                LawDocument = lawdocument;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lawdocument = await _context.LawDocuments.FindAsync(id);
            if (lawdocument != null)
            {
                LawDocument = lawdocument;
                _context.LawDocuments.Remove(LawDocument);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
