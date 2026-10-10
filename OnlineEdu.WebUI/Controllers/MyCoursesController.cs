using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.CourseRegisterDtos;
using OnlineEdu.WebUI.Models;
using OnlineEdu.WebUI.Services.TokenServices;
using System.Net;

namespace OnlineEdu.WebUI.Controllers
{
    [Authorize(Roles = "Student")]
    public class MyCoursesController : Controller
    {
        private readonly HttpClient _client;
        private readonly ITokenService _tokenService;

        public MyCoursesController(IHttpClientFactory clientFactory, ITokenService tokenService)
        {
            _client = clientFactory.CreateClient("EduClient");
            _tokenService = tokenService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = _tokenService.GetUserId;
            var values = await _client.GetFromJsonAsync<List<ResultCourseRegisterDto>>($"courseRegisters/GetMyCourses/{userId}") ?? new List<ResultCourseRegisterDto>();
            ViewBag.TotalCount = values.Count;
            var pageItems = PaginationViewModel.Paginate(values.OrderByDescending(x => x.CourseRegisterId).ToList(), page, 6, "/MyCourses/Index", out var pagination);
            ViewBag.Pagination = pagination;
            return View(pageItems);
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int id)
        {
            var dto = new CreateCourseRegisterDto { CourseId = id, AppUserId = _tokenService.GetUserId };
            var response = await _client.PostAsJsonAsync("courseRegisters", dto);

            // Bildirim kartında kursun adını göstermek için
            var course = await _client.GetFromJsonAsync<OnlineEdu.WebUI.DTOs.CourseDtos.ResultCourseDto>($"courses/{id}");
            TempData["EnrollCourseId"] = id;
            TempData["EnrollCourseName"] = course?.CourseName ?? "Kurs";

            if (response.IsSuccessStatusCode)
                TempData["EnrollStatus"] = "success";
            else if (response.StatusCode == HttpStatusCode.Conflict)
                TempData["EnrollStatus"] = "exists";
            else
                TempData["EnrollStatus"] = "error";

            return RedirectToAction(nameof(Index));
        }
    }
}
