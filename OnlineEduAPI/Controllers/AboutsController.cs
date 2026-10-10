using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.AboutDtos;
using OnlineEdu.Entity.Entities;

namespace OnlineEduAPI.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AboutsController(IGenericService<About> _aboutService, IMapper _mapper, IImageService _imageService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            var values = _aboutService.TGetList();
            return Ok(values);
        }

        [HttpGet("{id}")]

        public IActionResult GetById(int id) {
            var value = _aboutService.TGetById(id);
            return Ok(value);
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteById(int id)
        {
            var entity = _aboutService.TGetById(id);
            if (entity == null) return NotFound();
            _aboutService.TDelete(id);
            // CDN temizliği: kayıt silinince görseli de Cloudinary'den sil
            await _imageService.DeleteImageAsync(entity.ImageUrl1);
            await _imageService.DeleteImageAsync(entity.ImageUrl2);
            return Ok("Hakkımızda alanı silindi.");
        }

        [HttpPost]
        public IActionResult Create(CreateAboutDto createAboutDto)
        {
            var newValue = _mapper.Map<About>(createAboutDto);
            _aboutService.TCreate(newValue);
            return Ok("Yeni hakkımızda alanı oluşturuldu.");
        }

        [HttpPut]

        public async Task<IActionResult> Update(UpdateAboutDto updateAboutDto)
        {
            var newValue = _mapper.Map<About>(updateAboutDto);
            var old = _aboutService.TGetById(newValue.AboutId);
            var oldImageUrl1 = old?.ImageUrl1;
            var oldImageUrl2 = old?.ImageUrl2;
            _aboutService.TUpdate(newValue);
            // Görsel değiştiyse eskisini Cloudinary'den sil
            if (oldImageUrl1 != newValue.ImageUrl1) await _imageService.DeleteImageAsync(oldImageUrl1);
            if (oldImageUrl2 != newValue.ImageUrl2) await _imageService.DeleteImageAsync(oldImageUrl2);
            return Ok("Hakkımızda alanı güncellendi.");
        }

    }
}
