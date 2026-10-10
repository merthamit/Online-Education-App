using OnlineEdu.DTO.DTOs.MessageDtos;
using System.Security.Claims;
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
    public class CourseRegistersController(ICourseRegisterService _courseRegisterService, IMapper _mapper, ICourseService _courseService, Microsoft.AspNetCore.Identity.UserManager<AppUser> _userManager, MassTransit.IPublishEndpoint _publishEndpoint) : ControllerBase
    {
        [HttpGet("GetMyCourses/{userId}")]
        public async Task<IActionResult> GetMyCourses(int userId)
        {
            if (!User.IsInRole("Admin") && GetCurrentUserId() != userId)
            {
                return Forbid();
            }

            var value = _courseRegisterService.TGetAllWithCourseAndCategory(x => x.AppUserId == userId);
            var mappedValue = _mapper.Map<List<ResultCourseRegisterDto>>(value);
            return Ok(mappedValue);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterToCourse(CreateCourseRegisterDto createCourseRegisterDto)
        {
            if (!User.IsInRole("Admin"))
            {
                createCourseRegisterDto.AppUserId = GetCurrentUserId();
            }

            var alreadyRegistered = _courseRegisterService.TFilteredCount(x => x.AppUserId == createCourseRegisterDto.AppUserId && x.CourseId == createCourseRegisterDto.CourseId) > 0;
            if (alreadyRegistered)
            {
                return Conflict("Bu kursa zaten kayıtlısınız.");
            }

            var newCourseRegister = _mapper.Map<CourseRegister>(createCourseRegisterDto);
            _courseRegisterService.TCreate(newCourseRegister);

            // RabbitMQ: kayıt onay e-postası arka planda gönderilir
            var user = await _userManager.FindByIdAsync(createCourseRegisterDto.AppUserId.ToString());
            var course = _courseService.TGetById(createCourseRegisterDto.CourseId);
            if (user?.Email != null && course != null)
            {
                await _publishEndpoint.Publish(new SendEmailCommand
                {
                    To = user.Email,
                    Subject = $"Kursa kaydınız tamamlandı: {course.CourseName}",
                    Body = $"Merhaba {user.FirstName},\n\n\"{course.CourseName}\" kursuna kaydınız tamamlandı. Kayıt Olduğum Kurslar sayfasından videolara erişebilirsiniz."
                });
            }
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

        private int GetCurrentUserId()
        {
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;
        }
    }
}
