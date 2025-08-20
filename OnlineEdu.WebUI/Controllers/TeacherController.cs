using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.UserDtos;
using OnlineEdu.WebUI.Services.UserServices;
using System.Threading.Tasks;

namespace OnlineEdu.WebUI.Controllers
{
    public class TeacherController : Controller
    {
        private readonly HttpClient _client;

        public TeacherController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }
        public async Task<IActionResult> Index()
        {
            var values = await _client.GetFromJsonAsync<List<ResultUserDto>>("users/teacherlist");
            return View(values);
        }
    }
}
