using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using System.Threading.Tasks;

namespace OnlineEduAPI.Controllers
{
    // Giriş yapmış her kullanıcı (profil fotoğrafı için öğrenciler dahil) yükleyebilir
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageUploadController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Lütfen bir dosya seçin.");

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("Görsel en fazla 5 MB olabilir.");

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("Lütfen bir görsel dosyası (JPG, PNG, WEBP) seçin.");

            var url = await _imageService.UploadImageAsync(file);
            
            if (url == null)
                return BadRequest("Resim yüklenirken bir hata oluştu.");

            return Ok(new { Url = url });
        }
    }
}