using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.UserDtos;
using OnlineEdu.WebUI.Helpers;
using OnlineEdu.WebUI.Services.TokenServices;
using System.Security.Claims;

namespace OnlineEdu.WebUI.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        private readonly HttpClient _client;
        private readonly ITokenService _tokenService;

        public SettingController(IHttpClientFactory clientFactory, ITokenService tokenService)
        {
            _client = clientFactory.CreateClient("EduClient");
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = _tokenService.GetUserId;
            var values = await _client.GetFromJsonAsync<UpdateProfileDto>($"users/GetMyProfile/{userId}");
            return View(values);
        }

        [HttpPost]
        public async Task<IActionResult> Index(UpdateProfileDto model, IFormFile? photo, bool removePhoto = false)
        {
            var userId = _tokenService.GetUserId;
            model.Id = userId;

            // Mevcut fotoğrafı koru; yeni dosya seçildiyse Cloudinary'ye yükle, kaldır dendiyse boşalt
            var current = await _client.GetFromJsonAsync<UpdateProfileDto>($"users/GetMyProfile/{userId}");
            model.ImageUrl = current?.ImageUrl;

            if (photo != null && photo.Length > 0)
            {
                if (photo.Length > 5 * 1024 * 1024 || !photo.ContentType.StartsWith("image/"))
                {
                    ViewBag.Error = "Lütfen en fazla 5 MB boyutunda bir görsel (JPG, PNG, WEBP) seçin.";
                    return View(model);
                }

                var uploadedUrl = await _client.UploadImageAsync(photo);
                if (uploadedUrl == null)
                {
                    ViewBag.Error = "Fotoğraf yüklenemedi, lütfen tekrar deneyin.";
                    return View(model);
                }
                model.ImageUrl = uploadedUrl;
            }
            else if (removePhoto)
            {
                model.ImageUrl = null;
            }

            var response = await _client.PutAsJsonAsync("users/UpdateMyProfile", model);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Success = string.IsNullOrEmpty(model.NewPassword)
                    ? "Profiliniz başarıyla güncellendi."
                    : "Profiliniz ve şifreniz başarıyla güncellendi.";

                // Sitede görünen adı güncelle
                var identity = (ClaimsIdentity)User.Identity!;
                var fullNameClaim = identity.FindFirst("fullName");
                if (fullNameClaim != null)
                {
                    identity.RemoveClaim(fullNameClaim);
                }
                identity.AddClaim(new Claim("fullName", model.FirstName + " " + model.LastName));
                await HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.Error = errorContent.Contains("Şifre")
                    ? "Şifre değiştirilemedi. Mevcut şifrenizi doğru girdiğinizden ve yeni şifrenin kurallara uyduğundan emin olun."
                    : "Profil güncellenirken bir hata oluştu, lütfen tekrar deneyin.";
            }

            model.CurrentPassword = null;
            model.NewPassword = null;
            return View(model);
        }
    }
}
