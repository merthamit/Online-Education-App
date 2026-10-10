using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.Models;

namespace OnlineEdu.WebUI.Controllers
{
    [Authorize(Roles = "Student")]
    public class TeacherApplicationController : Controller
    {
        private readonly HttpClient _client;

        public TeacherApplicationController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Application = await GetMyApplicationAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string expertise, string motivation, string? portfolioUrl)
        {
            var response = await _client.PostAsJsonAsync("TeacherApplications", new { expertise, motivation, portfolioUrl });
            if (response.IsSuccessStatusCode)
            {
                TempData["ApplicationSent"] = true;
                return RedirectToAction(nameof(Index));
            }

            var text = await response.Content.ReadAsStringAsync();
            ViewBag.Error = text.Length < 250 && !text.TrimStart().StartsWith("{") ? text : "Başvuru gönderilemedi, lütfen tekrar deneyin.";
            ViewBag.Expertise = expertise;
            ViewBag.Motivation = motivation;
            ViewBag.PortfolioUrl = portfolioUrl;
            ViewBag.Application = await GetMyApplicationAsync();
            return View();
        }

        private async Task<TeacherApplicationDto?> GetMyApplicationAsync()
        {
            var response = await _client.GetAsync("TeacherApplications/My");
            return response.StatusCode == System.Net.HttpStatusCode.OK
                ? await response.Content.ReadFromJsonAsync<TeacherApplicationDto>()
                : null;
        }
    }
}
