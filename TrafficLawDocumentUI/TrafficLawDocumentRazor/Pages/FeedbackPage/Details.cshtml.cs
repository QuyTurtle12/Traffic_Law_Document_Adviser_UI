using BussinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util.DTOs.ApiResponse;
using Util.DTOs.FeedbackDTOs;

namespace TrafficLawDocumentRazor.Pages.FeedbackPage
{
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public GetFeedbackDto Feedback { get; set; } = default!;
        public async Task OnGetAsync(Guid id)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("API");

            var response = await httpClient.GetAsync($"/api/feedback/{id}");
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<GetFeedbackDto>>();
            if (result.Data != null)
            {
                Feedback = result.Data;
            }
        }
    }
}
