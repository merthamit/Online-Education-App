using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.UserDtos;
using OnlineEdu.Entity.Entities;
using System.Threading.Tasks;

namespace OnlineEduAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, IJwtService _jwtService, IMapper _mapper, IImageService _imageService, MassTransit.IPublishEndpoint _publishEndpoint) : ControllerBase
    {

        [HttpPost("login")]

        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("Bu Email sisteme kayıtlı değil.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (!result.Succeeded)
            {
                return BadRequest("Kullanıcı adı veya şifre yanlış.");
            }

            var token = await _jwtService.CreateTokenAsync(user);

            return Ok(token);
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register(RegisterDto model)
        {
            var user = _mapper.Map<AppUser>(model);

            if (model.Password != model.ConfirmPassword)
            {
                return BadRequest(new[] { new IdentityError { Code = "PasswordMismatch", Description = "Şifreler birbiriyle eşleşmiyor." } });
            }

            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                await _userManager.AddToRoleAsync(user, "Student");

                // RabbitMQ: hoş geldin e-postası arka planda gönderilir
                await _publishEndpoint.Publish(new OnlineEdu.DTO.DTOs.MessageDtos.SendEmailCommand
                {
                    To = user.Email,
                    Subject = "OnlineEdu'ya hoş geldiniz!",
                    Body = $"Merhaba {user.FirstName},\n\nHesabınız oluşturuldu. Kurslarımıza göz atıp öğrenmeye hemen başlayabilirsiniz."
                });
                return Ok("Kullanıcı Kaydı Başarılı");
            }

            return BadRequest(ModelState);
        }

        // Sitede herkese açık istatistikler için sadece sayılar
        [HttpGet("Stats")]
        public async Task<IActionResult> Stats([FromServices] Microsoft.Extensions.Caching.Distributed.IDistributedCache cache)
        {
            // Sayılar sık değişmez; Redis'te 5 dakika tutulur
            const string key = "public_user_stats";
            var cached = await cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cached))
            {
                return Content(cached, "application/json");
            }

            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
            var students = await _userManager.GetUsersInRoleAsync("Student");
            var json = System.Text.Json.JsonSerializer.Serialize(new { teacherCount = teachers.Count, studentCount = students.Count });
            await cache.SetStringAsync(key, json, new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
            return Content(json, "application/json");
        }

        [HttpGet("TeacherList")]

        public async Task<IActionResult> TeacherList()
        {
            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
            return Ok(teachers);
        }

        // Öğrenci listesi (ad, e-posta) sadece admin'e açık
        [Authorize(Roles = "Admin")]
        [HttpGet("StudentList")]

        public async Task<IActionResult> StudentList()
        {
            var students = await _userManager.GetUsersInRoleAsync("Student");
            return Ok(students);
        }

        [HttpGet("Get4Teachers")]
        public async Task<IActionResult> Get4Teachers()
        {
            var users = await _userManager.Users.Include(x => x.TeacherSocials).ToListAsync();
            var teachers = users.Where(user => _userManager.IsInRoleAsync(user, "Teacher").Result).OrderByDescending(x => x.Id).Take(4).ToList();
            return Ok(teachers);
        }



        [Authorize]
        [HttpGet("GetMyProfile/{id}")]
        public async Task<IActionResult> GetMyProfile(int id)
        {
            if (!CanAccessUser(id)) return Forbid();

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound("User not found");
            return Ok(new OnlineEdu.DTO.DTOs.UserDtos.UpdateProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ImageUrl = user.ImageUrl
            });
        }

        [Authorize]
        [HttpPut("UpdateMyProfile")]
        public async Task<IActionResult> UpdateMyProfile(OnlineEdu.DTO.DTOs.UserDtos.UpdateProfileDto model)
        {
            if (!CanAccessUser(model.Id)) return Forbid();

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null) return NotFound("User not found");

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            var oldImageUrl = user.ImageUrl;
            user.ImageUrl = model.ImageUrl;

            await _userManager.UpdateAsync(user);

            if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword))
            {
                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    return BadRequest("Şifre değiştirilemedi. Lütfen mevcut şifrenizi doğru girdiğinizden emin olun.");
                }
            }

            // Profil fotoğrafı değiştiyse eskisini Cloudinary'den sil
            if (oldImageUrl != user.ImageUrl)
            {
                await _imageService.DeleteImageAsync(oldImageUrl);
            }

            return Ok("Profil güncellendi");
        }


        // Kullanıcı sadece kendi profiline erişebilir; Admin herkesinkine
        private bool CanAccessUser(int userId)
        {
            if (User.IsInRole("Admin")) return true;
            return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var currentId) && currentId == userId;
        }
    }
}
