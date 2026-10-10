using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.AboutDtos;
using OnlineEdu.WebUI.Helpers;
using System.Threading.Tasks;

namespace OnlineEdu.WebUI.Controllers
{
    public class AboutController : Controller
    {
        private readonly HttpClient _client;

        public AboutController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }
        public async Task<IActionResult> Index()
        {
            var values = await _client.GetFromJsonAsync<List<ResultAboutDto>>("abouts") ?? new List<ResultAboutDto>();

            // Sayfadaki istatistikler
            var stats = await _client.GetFromJsonAsync<System.Text.Json.JsonElement>("users/Stats");
            ViewBag.TeacherCount = stats.GetProperty("teacherCount").GetInt32();
            ViewBag.StudentCount = stats.GetProperty("studentCount").GetInt32();
            ViewBag.CourseCount = await _client.GetFromJsonAsync<int>("courses/GetCourseCount");
            ViewBag.CategoryCount = await _client.GetFromJsonAsync<int>("coursecategories/GetCourseCategoryCount");
            return View(values);
        }
    }
}
