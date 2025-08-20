using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.MessageDtos;
using OnlineEdu.WebUI.Helpers;
using System.Threading.Tasks;

namespace OnlineEdu.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]

    [Area("Admin")]
    public class MessageController : Controller
    {
        private readonly HttpClient _client;

        public MessageController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }
        public async Task<IActionResult> Index()
        {
            var values = await _client.GetFromJsonAsync<List<ResultMessageDto>>("messages");
            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            await _client.DeleteAsync($"messages/{id}");
            return RedirectToAction("Index");
        }

        public IActionResult CreateMessage()
        {
            return View();
        }

        public async Task<IActionResult> MessageDetail(int id)
        {
            var result = await _client.GetFromJsonAsync<ResultMessageDto>("Messages/" + id); 
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(CreateMessageDto createMessageDto)
        {
            await _client.PostAsJsonAsync("messages", createMessageDto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateMessage(int id)
        {
            var values = await _client.GetFromJsonAsync<UpdateMessageDto>($"messages/{id}");
            return View(values);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMessage(UpdateMessageDto updateMessageDto)
        {
            await _client.PutAsJsonAsync("messages", updateMessageDto);
            return RedirectToAction(nameof(Index));

        }

    }
}
