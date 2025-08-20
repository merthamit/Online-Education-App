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
    public class TestimonialsController(IGenericService<Testimonial> _testimonialService, IMapper _mapper) : ControllerBase
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

        public IActionResult DeleteById(int id)
        {
            _testimonialService.TDelete(id);
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

        public IActionResult Update(UpdateTestimonialDto updateTestimonialDto)
        {
            var newValue = _mapper.Map<Testimonial>(updateTestimonialDto);
            _testimonialService.TUpdate(newValue);
            return Ok("Testimonial alanı güncellendi.");
        }

    }
}
