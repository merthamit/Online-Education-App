using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.CourseCategoryDtos;
using OnlineEdu.Entity.Entities;

namespace OnlineEduAPI.Controllers
{
    [Authorize(Roles = "Admin, Teacher")]
    [Route("api/[controller]")]
    [ApiController]
    public class CourseCategoriesController(ICourseCategoryService _courseCategoryService, IMapper _mapper) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            var values = _courseCategoryService.TGetList();
            return Ok(values);
        }

        [AllowAnonymous]
        [HttpGet("GetCourseCategoryCount")]
        public IActionResult Count()
        {
            var value = _courseCategoryService.TCount();
            return Ok(value);
        }

        [HttpGet("{id}")]

        public IActionResult GetById(int id)
        {
            var value = _courseCategoryService.TGetById(id);
            return Ok(value);
        }

        [HttpDelete("{id}")]

        public IActionResult DeleteById(int id)
        {
            _courseCategoryService.TDelete(id);
            return Ok("Course Category alanı silindi.");
        }

        [HttpPost]
        public IActionResult Create(CreateCourseCategoryDto createCourseCategoryDto)
        {
            var newValue = _mapper.Map<CourseCategory>(createCourseCategoryDto);
            _courseCategoryService.TCreate(newValue);
            return Ok("Yeni Course Category alanı oluşturuldu.");
        }

        [HttpPut]

        public IActionResult Update(UpdateCourseCategoryDto updateCourseCategoryDto)
        {
            var newValue = _mapper.Map<CourseCategory>(updateCourseCategoryDto);
            _courseCategoryService.TUpdate(newValue);
            return Ok("Course Category alanı güncellendi.");
        }

        [HttpGet("ShowOnHome/{id}")]
        public IActionResult ShowOnHome(int id)
        {
            _courseCategoryService.TShowOnHome(id);
            return Ok("Gösteriliyor.");
        }

        [HttpGet("DontShowOnHome/{id}")]
        public IActionResult DontShowOnHome(int id)
        {
            _courseCategoryService.TDontShowOnHome(id);
            return Ok("Gösterilmiyor.");
        }

        [AllowAnonymous]
        [HttpGet("GetActiveCategories")]

        public IActionResult GetActiveCategories()
        {
            var values = _courseCategoryService.TGetFilteredList(x => x.IsShown == true);
            return Ok(values);
        }

    }
}
