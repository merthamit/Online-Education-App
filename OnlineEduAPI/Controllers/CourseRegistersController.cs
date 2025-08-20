using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.CourseRegisterDtos;
using OnlineEdu.Entity.Entities;
using System.Threading.Tasks;

namespace OnlineEduAPI.Controllers
{
    [Authorize(Roles = "Admin, Student")]
    [Route("api/[controller]")]
    [ApiController]
    public class CourseRegistersController(ICourseRegisterService _courseRegisterService, IMapper _mapper) : ControllerBase
    {
        [HttpGet("GetMyCourses/{userId}")]
        public async Task<IActionResult> GetMyCourses(int userId)
        {
            var value = _courseRegisterService.TGetAllWithCourseAndCategory(x => x.AppUserId == userId);
            var mappedValue = _mapper.Map<List<ResultCourseRegisterDto>>(value);
            return Ok(mappedValue);
        }

        [HttpPost]
        public IActionResult RegisterToCourse(CreateCourseRegisterDto createCourseRegisterDto)
        {
            var newCourseRegister = _mapper.Map<CourseRegister>(createCourseRegisterDto);
            _courseRegisterService.TCreate(newCourseRegister);
            return Ok("Başarıyla Kayıt Olundu.");
        }

        [HttpPut]

        public IActionResult UpdateCourseRegister(UpdateCourseRegisterDto updateCourseRegisterDto)
        {
            var updateCourseRegister = _mapper.Map<CourseRegister>(updateCourseRegisterDto);
            _courseRegisterService.TUpdate(updateCourseRegister);
            return Ok("Başarıyla Güncellendi.");
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var value = _courseRegisterService.TGetById(id);
            var mapped = _mapper.Map<ResultCourseRegisterDto>(value);    
            return Ok(value);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCourseRegister(int id)
        {
            _courseRegisterService.TDelete(id);
            return Ok("Başarıyla silindi.");
        }
    }
}
