using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.ContactDtos;
using OnlineEdu.WebUI.Helpers;
using System.Threading.Tasks;

namespace OnlineEdu.WebUI.Controllers
{
    public class ContactController : Controller
    {
        private readonly HttpClient _client;

        public ContactController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }
        public async Task<IActionResult> Index()
        {
            var values = await _client.GetFromJsonAsync<List<ResultContactDto>>("contact");
            ViewBag.map = values?.Select(x => x.MapUrl).FirstOrDefault();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(OnlineEdu.WebUI.DTOs.MessageDtos.CreateMessageDto model)
        {
            var response = await _client.PostAsJsonAsync("messages", model);
            if (response.IsSuccessStatusCode)
            {
                TempData["MessageSent"] = "true";
            }
            return RedirectToAction("Index");
        }


    }
}
