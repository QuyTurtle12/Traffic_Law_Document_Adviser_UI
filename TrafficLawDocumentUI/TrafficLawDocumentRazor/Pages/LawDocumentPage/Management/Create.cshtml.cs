using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BussinessObject;

namespace TrafficLawDocumentRazor.Pages.LawDocumentPage.Management
{
    public class CreateModel : PageModel
    {
        private readonly BussinessObject.TrafficLawDocumentDbContext _context;

        public CreateModel(BussinessObject.TrafficLawDocumentDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["CategoryId"] = new SelectList(_context.DocumentCategories, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public LawDocument LawDocument { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.LawDocuments.Add(LawDocument);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
