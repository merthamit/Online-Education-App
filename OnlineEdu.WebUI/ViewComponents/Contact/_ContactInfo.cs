using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.ContactDtos;
using OnlineEdu.WebUI.Helpers;

namespace OnlineEdu.WebUI.ViewComponents.Contact
{
    public class _ContactInfo : ViewComponent
    {
        private readonly HttpClient _client;

        public _ContactInfo(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _client.GetFromJsonAsync<List<ResultContactDto>>("contact");
            return View(result);
        }
    }
}
