using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.TeacherSocialDtos;
using OnlineEdu.WebUI.Services.TokenServices;

namespace OnlineEdu.WebUI.Areas.Teacher.Controllers
{
    [Authorize(Roles = "Teacher")]
    [Area("Teacher")]
    public class MySocialMediaController : Controller
    {
        private readonly HttpClient _client;
        private readonly ITokenService _tokenService;


        public MySocialMediaController( ITokenService tokenService, IHttpClientFactory clientFactory)
        {
            _tokenService = tokenService;
            _client = clientFactory.CreateClient("EduClient");
        }

        public async Task<IActionResult> Index()
        {
            var userId = _tokenService.GetUserId;
            var values = await _client.GetFromJsonAsync<List<ResultTeacherSocialDto>>($"teachersocials/GetSocialByTeacherId/{userId}");
            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteTeacherSocial(int id)
        {
            await _client.DeleteAsync($"teachersocials/{id}");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CreateTeacherSocial()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateTeacherSocial(CreateTeacherSocialDto createTeacherSocialDto)
        {
            var userId = _tokenService.GetUserId;
            createTeacherSocialDto.TeacherId = userId;
            await _client.PostAsJsonAsync("teachersocials", createTeacherSocialDto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpdateTeacherSocial(int id)
        {
            var values = await _client.GetFromJsonAsync<UpdateTeacherSocialDto>($"teachersocials/{id}");
            return View(values);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTeacherSocial(UpdateTeacherSocialDto updateTeacherSocialMedia)
        {
            var userId = _tokenService.GetUserId;
            updateTeacherSocialMedia.TeacherId = userId;
            await _client.PutAsJsonAsync("teachersocials", updateTeacherSocialMedia);
            return RedirectToAction(nameof(Index));
        }
    }
}
