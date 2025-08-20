using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.CourseCategoryDtos;
using OnlineEdu.WebUI.Helpers;
using System.Threading.Tasks;

namespace OnlineEdu.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]

    [Area("Admin")]
    public class CourseCategoryController : Controller
    {
        private readonly HttpClient _client;

        public CourseCategoryController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }

        public async Task<IActionResult> Index()
        {
            var values = await _client.GetFromJsonAsync<List<ResultCourseCategoryDto>>("coursecategories");
            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCourseCategory(int id)
        {
            await _client.DeleteAsync($"coursecategories/{id}");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CreateCourseCategory()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateCourseCategory(CreateCourseCategoryDto createCourseCategoryDto)
        {
            await _client.PostAsJsonAsync("coursecategories", createCourseCategoryDto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCourseCategory(int id)
        {
            var values = await _client.GetFromJsonAsync<UpdateCourseCategoryDto>($"coursecategories/{id}");
            return View(values);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCourseCategory(UpdateCourseCategoryDto updateCourseCategoryDto)
        {
            await _client.PutAsJsonAsync("coursecategories", updateCourseCategoryDto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DontShowOnHome(int id)
        {
            await _client.GetAsync($"coursecategories/DontShowOnHome/{id}");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ShowOnHome(int id)
        {
            await _client.GetAsync($"coursecategories/ShowOnHome/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
