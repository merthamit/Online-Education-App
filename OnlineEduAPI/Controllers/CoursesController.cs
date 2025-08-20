using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.CourseDtos;
using OnlineEdu.Entity.Entities;

namespace OnlineEduAPI.Controllers
{
    [Authorize(Roles = "Admin, Teacher, Student")]
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController(ICourseService _courseService, IMapper _mapper) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            var values = _courseService.TGetCoursesWithCategories();
            return Ok(values);
        }

        [AllowAnonymous]
        [HttpGet("GetCourseCount")]
        public IActionResult Count()
        {
            var value = _courseService.TCount();
            return Ok(value);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]

        public IActionResult GetById(int id)
        {
            var value = _courseService.TGetById(id);
            return Ok(value);
        }

        [HttpDelete("{id}")]

        public IActionResult DeleteById(int id)
        {
            _courseService.TDelete(id);
            return Ok("Course alanı silindi.");
        }

        [HttpPost]
        public IActionResult Create(CreateCourseDto createCourseDto)
        {
            var newValue = _mapper.Map<Course>(createCourseDto);
            _courseService.TCreate(newValue);
            return Ok("Yeni Course alanı oluşturuldu.");
        }

        [HttpPut]

        public IActionResult Update(UpdateCourseDto updateCourseDto)
        {
            var newValue = _mapper.Map<Course>(updateCourseDto);
            _courseService.TUpdate(newValue);
            return Ok("Course alanı güncellendi.");
        }

        [HttpGet("ShowOnHome/{id}")]
        public IActionResult ShowOnHome(int id)
        {
            _courseService.TShowOnHome(id);
            return Ok("Gösteriliyor.");
        }

        [HttpGet("DontShowOnHome/{id}")]
        public IActionResult DontShowOnHome(int id)
        {
            _courseService.TDontShowOnHome(id);
            return Ok("Gösterilmiyor.");
        }

        [AllowAnonymous]
        [HttpGet("GetActiveCourses")]

        public IActionResult GetActiveCourses()
        {
            var values = _courseService.TGetFilteredList(x => x.IsShown == true);
            return Ok(values);
        }

        [AllowAnonymous]
        [HttpGet("GetCoursesByCategoryId/{id}")]

        public IActionResult GetCoursesByCategoryId(int id)
        {
            var values = _courseService.TGetCoursesWithCategories(x => x.CourseCategoryId == id);
            return Ok(values);
        }

        [HttpGet("GetCoursesByTeacherId/{id}")]
        public IActionResult GetCoursesByTeacherId(int id)
        {
            var values = _courseService.TGetCourseByTeacherId(id);
            var mapper = _mapper.Map<List<ResultCourseDto>>(values);
            return Ok(values);
        }
    }
}
