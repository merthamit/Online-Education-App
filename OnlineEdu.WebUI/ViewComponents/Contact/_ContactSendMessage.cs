using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.Helpers;

namespace OnlineEdu.WebUI.ViewComponents.Contact
{
    public class _ContactSendMessage : ViewComponent
    {
        private readonly HttpClient _client;

        public _ContactSendMessage(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
