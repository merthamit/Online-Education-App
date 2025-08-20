using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.CourseVideoDtos;
using OnlineEdu.Entity.Entities;

namespace OnlineEduAPI.Controllers
{
    [Authorize(Roles = "Admin, Teacher, Student")]
    [Route("api/[controller]")]
    [ApiController]
    public class CourseVideosController(IGenericService<CourseVideo> _courseVideoService, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var values = _courseVideoService.TGetList();
            return Ok(values);
        }
        [HttpGet("GetCourseVideosById/{id}")]
        public IActionResult GetCourseVideosById(int id)
        {
            var values = _courseVideoService.TGetFilteredList(x => x.CourseId == id);
            return Ok(values);
        }

        [HttpGet("{id}")]

        public IActionResult GetById(int id)
        {
            var value = _courseVideoService.TGetById(id);
            return Ok(value);
        }

        [HttpDelete("{id}")]

        public IActionResult DeleteById(int id)
        {
            _courseVideoService.TDelete(id);
            return Ok("Course Video alanı silindi.");
        }

        [HttpPost]
        public IActionResult Create(CreateCourseVideoDto createCourseVideo)
        {
            var newValue = _mapper.Map<CourseVideo>(createCourseVideo);
            _courseVideoService.TCreate(newValue);
            return Ok("Yeni Course Video alanı oluşturuldu.");
        }

        [HttpPut]

        public IActionResult Update(UpdateCourseVideo updateCourseVideo)
        {
            var newValue = _mapper.Map<CourseVideo>(updateCourseVideo);
            _courseVideoService.TUpdate(newValue);
            return Ok("Course Video alanı güncellendi.");
        }
    }
}
