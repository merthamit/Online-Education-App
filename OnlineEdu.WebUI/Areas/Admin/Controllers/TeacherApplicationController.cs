using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.Models;

namespace OnlineEdu.WebUI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class TeacherApplicationController : Controller
    {
        private readonly HttpClient _client;

        public TeacherApplicationController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }

        public async Task<IActionResult> Index()
        {
            var values = await _client.GetFromJsonAsync<List<TeacherApplicationDto>>("TeacherApplications") ?? new();
            return View(values);
        }

        [HttpPost]
        public async Task<IActionResult> Approve(int id, string? note)
        {
            var response = await _client.PostAsJsonAsync($"TeacherApplications/{id}/Approve", new { note });
            TempData["AppResult"] = response.IsSuccessStatusCode ? "Başvuru onaylandı; kullanıcı artık eğitmen ve bilgilendirme e-postası gönderildi." : "Başvuru onaylanamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Reject(int id, string? note)
        {
            var response = await _client.PostAsJsonAsync($"TeacherApplications/{id}/Reject", new { note });
            TempData["AppResult"] = response.IsSuccessStatusCode ? "Başvuru reddedildi ve kullanıcıya bilgi verildi." : "Başvuru reddedilemedi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
