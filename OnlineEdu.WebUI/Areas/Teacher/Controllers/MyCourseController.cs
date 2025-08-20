using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineEdu.WebUI.DTOs.CourseCategoryDtos;
using OnlineEdu.WebUI.DTOs.CourseDtos;
using OnlineEdu.WebUI.DTOs.CourseVideoDtos;
using OnlineEdu.WebUI.Services.TokenServices;

namespace OnlineEdu.WebUI.Areas.Teacher.Controllers
{
    [Authorize(Roles = "Teacher")]
    [Area("Teacher")]
    public class MyCourseController : Controller
    {
        private readonly HttpClient _client;
        private readonly ITokenService _tokenService;

        public MyCourseController(ITokenService tokenService, IHttpClientFactory clientFactory)
        {
            _tokenService = tokenService;
            _client = clientFactory.CreateClient("EduClient");
        }

        public async Task CourseCategoryDropDown()
        {
            var categoryList = await _client.GetFromJsonAsync<List<ResultCourseCategoryDto>>("coursecategories");
            List<SelectListItem> categories = (from x in categoryList select new SelectListItem { Text = x.CategoryName, Value = x.CourseCategoryId.ToString() }).ToList();
            ViewBag.categories = categories;
        }

        public  async Task<IActionResult> Index()
        {
            var userId = _tokenService.GetUserId;
            var values = await _client.GetFromJsonAsync<List<ResultCourseDto>>($"courses/GetCoursesByTeacherId/{userId}");
            return View(values);
        }

        public async Task<IActionResult> CreateCourse()
        {
            await CourseCategoryDropDown();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CourseVideos(int id)
        {
            var values = await _client.GetFromJsonAsync<List<ResultCourseVideoDto>>($"coursevideos/GetCourseVideosById/{id}");
            TempData["courseId"] = id;
            ViewBag.courseName = values.Select(x => x.Course.CourseName).FirstOrDefault();
            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> CreateVideo()
        {
            var courseId = (int)TempData["courseId"];
            var course = await _client.GetFromJsonAsync<ResultCourseDto>($"courses/{courseId}");
            ViewBag.courseName = course.CourseName;
            ViewBag.courseId = course.CourseId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateVideo(CreateCourseVideoDto createCourseVideoDto)
        {
            await _client.PostAsJsonAsync<CreateCourseVideoDto>("coursevideos" ,createCourseVideoDto);
            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public async Task<IActionResult> CreateCourse(CreateCourseDto createCourseDto)
        {
            var userId = _tokenService.GetUserId;
            createCourseDto.AppUserId = userId;
            createCourseDto.IsShown = false;
            await _client.PostAsJsonAsync("courses", createCourseDto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateCourse(int id) {
            await CourseCategoryDropDown();
            var value = await _client.GetFromJsonAsync<UpdateCourseDto>($"courses/{id}");
            return View(value);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateCourse(UpdateCourseDto updateCourseDto)
        {
            var userId = _tokenService.GetUserId;
            updateCourseDto.AppUserId = userId;
            await _client.PutAsJsonAsync("courses", updateCourseDto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteCourse(int id) {
            await _client.DeleteAsync($"courses/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
