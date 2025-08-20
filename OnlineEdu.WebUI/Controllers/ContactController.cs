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
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(CreateContactDto model)
        {
            await _client.PostAsJsonAsync<CreateContactDto>("messages", model);
            return NoContent();
        }

        public async Task<PartialViewResult> ContactMap()
        {
            var values = await _client.GetFromJsonAsync<List<ResultContactDto>>("contact");
            ViewBag.map = values.Select(x => x.MapUrl).FirstOrDefault();
            return PartialView();
        }
    }
}
