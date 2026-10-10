using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.BlogDtos;
using OnlineEdu.WebUI.DTOs.CourseDtos;
using OnlineEdu.WebUI.DTOs.MessageDtos;
using OnlineEdu.WebUI.DTOs.SubscriberDtos;
using OnlineEdu.WebUI.DTOs.UserDtos;

namespace OnlineEdu.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly HttpClient _client;

        public DashboardController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }

        public async Task<IActionResult> Index()
        {
            // İstatistikleri paralel çek
            var coursesTask = _client.GetFromJsonAsync<List<ResultCourseDto>>("courses");
            var blogsTask = _client.GetFromJsonAsync<List<ResultBlogDto>>("blogs");
            var teachersTask = _client.GetFromJsonAsync<List<ResultUserDto>>("users/TeacherList");
            var studentsTask = _client.GetFromJsonAsync<List<ResultUserDto>>("users/StudentList");
            var messagesTask = _client.GetFromJsonAsync<List<ResultMessageDto>>("messages");
            var subscribersTask = _client.GetFromJsonAsync<List<ResultSubscriberDto>>("subscribers");
            await Task.WhenAll(coursesTask, blogsTask, teachersTask, studentsTask, messagesTask, subscribersTask);

            var courses = coursesTask.Result ?? new();
            var messages = messagesTask.Result ?? new();

            ViewBag.CourseCount = courses.Count;
            ViewBag.BlogCount = blogsTask.Result?.Count ?? 0;
            ViewBag.TeacherCount = teachersTask.Result?.Count ?? 0;
            ViewBag.StudentCount = studentsTask.Result?.Count ?? 0;
            ViewBag.MessageCount = messages.Count;
            ViewBag.SubscriberCount = subscribersTask.Result?.Count ?? 0;
            ViewBag.LatestCourses = courses.OrderByDescending(x => x.CourseId).Take(5).ToList();
            ViewBag.PendingApplications = await _client.GetFromJsonAsync<int>("TeacherApplications/PendingCount");
            ViewBag.LatestMessages = messages.OrderByDescending(x => x.MessageId).Take(5).ToList();
            return View();
        }
    }
}
