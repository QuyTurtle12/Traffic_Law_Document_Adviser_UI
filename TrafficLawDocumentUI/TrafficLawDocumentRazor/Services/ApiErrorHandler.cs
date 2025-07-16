using Microsoft.AspNetCore.Mvc.RazorPages;
using Util.DTOs.ApiResponse;

namespace TrafficLawDocumentRazor.Services
{
    public static class ApiErrorHandler
    {
        public static async Task HandleErrorResponse(PageModel page, HttpResponseMessage response, string defaultErrorMessage)
        {
            try
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                page.TempData["ToastMessage"] = errorResponse?.ErrorMessage ?? defaultErrorMessage;
                page.TempData["ToastType"] = "error";
            }
            catch (Exception)
            {
                page.TempData["ToastMessage"] = defaultErrorMessage;
                page.TempData["ToastType"] = "error";
            }
        }
    }
}
