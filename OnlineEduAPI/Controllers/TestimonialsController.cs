using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.TestimonialDtos;
using OnlineEdu.Entity.Entities;

namespace OnlineEduAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class TestimonialsController(IGenericService<Testimonial> _testimonialService, IMapper _mapper, IImageService _imageService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]

        public IActionResult Get()
        {
            var value = _testimonialService.TGetList();
            return Ok(value);
        }

        [AllowAnonymous]
        [HttpGet("GetTestimonialCount")]
        public IActionResult Count()
        {
            var value = _testimonialService.TCount();
            return Ok(value);
        }

        [HttpGet("{id}")]

        public IActionResult GetById(int id)
        {
            var value = _testimonialService.TGetById(id);
            return Ok(value);
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteById(int id)
        {
            var entity = _testimonialService.TGetById(id);
            if (entity == null) return NotFound();
            _testimonialService.TDelete(id);
            // CDN temizliği: kayıt silinince görseli de Cloudinary'den sil
            await _imageService.DeleteImageAsync(entity.ImageUrl);
            return Ok("Testimonial alanı silindi.");
        }

        [HttpPost]
        public IActionResult Create(CreateTestimonialDto createTestimonialDto)
        {
            var newValue = _mapper.Map<Testimonial>(createTestimonialDto);
            _testimonialService.TCreate(newValue);
            return Ok("Yeni Testimonial alanı oluşturuldu.");
        }

        [HttpPut]

        public async Task<IActionResult> Update(UpdateTestimonialDto updateTestimonialDto)
        {
            var newValue = _mapper.Map<Testimonial>(updateTestimonialDto);
            var old = _testimonialService.TGetById(newValue.TestimonialId);
            var oldImageUrl = old?.ImageUrl;
            _testimonialService.TUpdate(newValue);
            // Görsel değiştiyse eskisini Cloudinary'den sil
            if (oldImageUrl != newValue.ImageUrl) await _imageService.DeleteImageAsync(oldImageUrl);
            return Ok("Testimonial alanı güncellendi.");
        }

    }
}
