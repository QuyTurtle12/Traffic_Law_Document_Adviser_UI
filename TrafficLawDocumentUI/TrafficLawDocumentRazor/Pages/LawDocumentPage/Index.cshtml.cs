using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BussinessObject;

namespace TrafficLawDocumentRazor.Pages.LawDocumentPage
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public IndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public IList<LawDocument> LawDocument { get;set; } = default!;

        public async Task OnGetAsync()
        {
            
        }
    }
}
