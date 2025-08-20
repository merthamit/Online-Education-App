using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.TeacherSocialDtos;
using OnlineEdu.Entity.Entities;

namespace OnlineEduAPI.Controllers
{
    [Authorize(Roles = "Admin, Teacher")]
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherSocialsController(IGenericService<TeacherSocial> _teacherSocialService, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var values = _teacherSocialService.TGetList();
            return Ok(values);
        }

        [AllowAnonymous]
        [HttpGet("GetSocialByTeacherId/{id}")]
        public IActionResult GetSocialByTeacherId(int id)
        {
            var values = _teacherSocialService.TGetFilteredList(x => x.TeacherId == id);
            return Ok(values);
        }

        [HttpGet("{id}")]

        public IActionResult GetById(int id)
        {
            var value = _teacherSocialService.TGetById(id);
            return Ok(value);
        }

        [HttpDelete("{id}")]

        public IActionResult DeleteById(int id)
        {
            _teacherSocialService.TDelete(id);
            return Ok("Social Media alanı silindi.");
        }

        [HttpPost]
        public IActionResult Create(CreateTeacherSocialDto createTeacherSocialDto)
        {
            var newValue = _mapper.Map<TeacherSocial>(createTeacherSocialDto);
            _teacherSocialService.TCreate(newValue);
            return Ok("Yeni Social Media alanı oluşturuldu.");
        }

        [HttpPut]

        public IActionResult Update(UpdateTeacherSocialDto updateSocialMediaDto)
        {
            var newValue = _mapper.Map<TeacherSocial>(updateSocialMediaDto);
            _teacherSocialService.TUpdate(newValue);
            return Ok("Social Media alanı güncellendi.");
        }
    }
}
