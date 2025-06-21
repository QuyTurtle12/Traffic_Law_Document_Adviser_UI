using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BussinessObject;

namespace TrafficLawDocumentRazor.Pages.LawDocumentPage
{
    public class EditModel : PageModel
    {
        private readonly BussinessObject.TrafficLawDocumentDbContext _context;

        public EditModel(BussinessObject.TrafficLawDocumentDbContext context)
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

            var lawdocument =  await _context.LawDocuments.FirstOrDefaultAsync(m => m.Id == id);
            if (lawdocument == null)
            {
                return NotFound();
            }
            LawDocument = lawdocument;
           ViewData["CategoryId"] = new SelectList(_context.DocumentCategories, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(LawDocument).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LawDocumentExists(LawDocument.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool LawDocumentExists(Guid id)
        {
            return _context.LawDocuments.Any(e => e.Id == id);
        }
    }
}
