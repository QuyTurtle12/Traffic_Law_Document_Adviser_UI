using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObject;

namespace TrafficLawDocumentRazor.Pages.LawDocumentPage
{
    public class IndexModel : PageModel
    {
        private readonly BussinessObject.TrafficLawDocumentDbContext _context;

        public IndexModel(BussinessObject.TrafficLawDocumentDbContext context)
        {
            _context = context;
        }

        public IList<LawDocument> LawDocument { get;set; } = default!;

        public async Task OnGetAsync()
        {
            LawDocument = await _context.LawDocuments
                .Include(l => l.Category).ToListAsync();
        }
    }
}
