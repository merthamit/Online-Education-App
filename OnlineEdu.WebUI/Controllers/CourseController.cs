using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.CourseDtos;
using OnlineEdu.WebUI.Helpers;
using System.Threading.Tasks;

namespace OnlineEdu.WebUI.Controllers
{
    public class CourseController : Controller
    {
        private readonly HttpClient _client;

        public CourseController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }
        public async Task<IActionResult> Index()
        {
            var courses = await _client.GetFromJsonAsync<List<ResultCourseDto>>("courses");
            return View(courses);
        }

        public async Task<IActionResult> GetCoursesByCategoryId(int id)
        {
            var courses = await _client.GetFromJsonAsync<List<ResultCourseDto>>($"courses/GetCoursesByCategoryId/{id}");
            ViewBag.category = courses.Any() ? courses.First().Category.CategoryName : "Kategori yok";
            return View(courses);
        }
    }
}
