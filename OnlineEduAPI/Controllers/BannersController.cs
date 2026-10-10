using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.AboutDtos;
using OnlineEdu.DTO.DTOs.BannerDtos;
using OnlineEdu.Entity.Entities;

namespace OnlineEduAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController(IGenericService<Banner> _bannerService, IMapper _mapper, IImageService _imageService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]

        public IActionResult Get()
        {
            var value = _bannerService.TGetList();
            return Ok(value);
        }


        [HttpGet("{id}")]

        public IActionResult GetById(int id)
        {
            var value = _bannerService.TGetById(id);
            return Ok(value);
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteById(int id)
        {
            var entity = _bannerService.TGetById(id);
            if (entity == null) return NotFound();
            _bannerService.TDelete(id);
            // CDN temizliği: kayıt silinince görseli de Cloudinary'den sil
            await _imageService.DeleteImageAsync(entity.ImageUrl);
            return Ok("Banner alanı silindi.");
        }

        [HttpPost]
        public IActionResult Create(CreateBannerDto createBannerDto)
        {
            var newValue = _mapper.Map<Banner>(createBannerDto);
            _bannerService.TCreate(newValue);
            return Ok("Yeni Banner alanı oluşturuldu.");
        }

        [HttpPut]

        public async Task<IActionResult> Update(UpdateBannerDto updateBannerDto)
        {
            var newValue = _mapper.Map<Banner>(updateBannerDto);
            var old = _bannerService.TGetById(newValue.BannerId);
            var oldImageUrl = old?.ImageUrl;
            _bannerService.TUpdate(newValue);
            // Görsel değiştiyse eskisini Cloudinary'den sil
            if (oldImageUrl != newValue.ImageUrl) await _imageService.DeleteImageAsync(oldImageUrl);
            return Ok("Banner alanı güncellendi.");
        }
    }
}
