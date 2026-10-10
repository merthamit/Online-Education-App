using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.MessageDtos;
using OnlineEdu.Entity.Entities;
using System.Security.Claims;

namespace OnlineEduAPI.Controllers
{
    public record CreateTeacherApplicationDto(string Expertise, string Motivation, string? PortfolioUrl);
    public record ReviewTeacherApplicationDto(string? Note);

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherApplicationsController(
        IGenericService<TeacherApplication> _applicationService,
        UserManager<AppUser> _userManager,
        IPublishEndpoint _publishEndpoint) : ControllerBase
    {
        private int CurrentUserId => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        private static object ToDto(TeacherApplication a) => new
        {
            a.TeacherApplicationId,
            a.AppUserId,
            a.Expertise,
            a.Motivation,
            a.PortfolioUrl,
            Status = a.Status.ToString(),
            a.AdminNote,
            a.CreatedAt,
            a.ReviewedAt,
            UserFullName = a.AppUser != null ? $"{a.AppUser.FirstName} {a.AppUser.LastName}" : "",
            UserEmail = a.AppUser?.Email,
            UserImageUrl = a.AppUser?.ImageUrl
        };

        // Öğrencinin son başvurusu (yoksa 204)
        [HttpGet("My")]
        public IActionResult My()
        {
            var application = _applicationService.TGetFilteredList(x => x.AppUserId == CurrentUserId)
                .OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            return application == null ? NoContent() : Ok(ToDto(application));
        }

        // Öğrenci eğitmenlik başvurusu yapar
        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTeacherApplicationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Expertise) || dto.Expertise.Length > 200)
                return BadRequest("Uzmanlık alanınızı girin (en fazla 200 karakter).");
            if (string.IsNullOrWhiteSpace(dto.Motivation) || dto.Motivation.Trim().Length < 30)
                return BadRequest("Kendinizi ve neden eğitmen olmak istediğinizi en az 30 karakterle anlatın.");
            if (dto.Motivation.Length > 2000)
                return BadRequest("Açıklama en fazla 2000 karakter olabilir.");
            if (!string.IsNullOrWhiteSpace(dto.PortfolioUrl) && !Uri.TryCreate(dto.PortfolioUrl, UriKind.Absolute, out _))
                return BadRequest("Portfolyo / LinkedIn bağlantısı geçerli bir adres olmalıdır.");

            var hasPending = _applicationService.TFilteredCount(x => x.AppUserId == CurrentUserId && x.Status == ApplicationStatus.Pending) > 0;
            if (hasPending)
                return Conflict("Değerlendirme bekleyen bir başvurunuz zaten var.");

            _applicationService.TCreate(new TeacherApplication
            {
                AppUserId = CurrentUserId,
                Expertise = dto.Expertise.Trim(),
                Motivation = dto.Motivation.Trim(),
                PortfolioUrl = string.IsNullOrWhiteSpace(dto.PortfolioUrl) ? null : dto.PortfolioUrl.Trim(),
                Status = ApplicationStatus.Pending,
                CreatedAt = DateTime.Now
            });
            // RabbitMQ: yöneticilere yeni başvuru bildirimi
            var applicant = await _userManager.FindByIdAsync(CurrentUserId.ToString());
            foreach (var admin in await _userManager.GetUsersInRoleAsync("Admin"))
            {
                if (admin.Email == null) continue;
                await _publishEndpoint.Publish(new SendEmailCommand
                {
                    To = admin.Email,
                    Subject = "Yeni eğitmenlik başvurusu",
                    Body = $"{applicant?.FirstName} {applicant?.LastName} ({applicant?.Email}) eğitmenlik başvurusu yaptı. Uzmanlık: {dto.Expertise}. Yönetim panelinden değerlendirebilirsiniz."
                });
            }
            return Ok("Başvurunuz alındı.");
        }

        // ---------------- Admin ----------------

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _applicationService.TGetList()
                .OrderBy(x => x.Status)              // önce bekleyenler
                .ThenByDescending(x => x.CreatedAt)
                .Select(ToDto)
                .ToList();
            return Ok(list);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("PendingCount")]
        public IActionResult PendingCount()
        {
            return Ok(_applicationService.TFilteredCount(x => x.Status == ApplicationStatus.Pending));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/Approve")]
        public async Task<IActionResult> Approve(int id, ReviewTeacherApplicationDto dto)
        {
            var application = _applicationService.TGetById(id);
            if (application == null) return NotFound();
            if (application.Status != ApplicationStatus.Pending) return Conflict("Bu başvuru zaten değerlendirilmiş.");

            var user = await _userManager.FindByIdAsync(application.AppUserId.ToString());
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            // Öğrenci rolünü eğitmen rolüyle değiştir
            if (!await _userManager.IsInRoleAsync(user, "Teacher"))
                await _userManager.AddToRoleAsync(user, "Teacher");
            if (await _userManager.IsInRoleAsync(user, "Student"))
                await _userManager.RemoveFromRoleAsync(user, "Student");

            application.Status = ApplicationStatus.Approved;
            application.AdminNote = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim();
            application.ReviewedAt = DateTime.Now;
            _applicationService.TUpdate(application);

            // RabbitMQ: öğrenciye bilgilendirme e-postası
            await _publishEndpoint.Publish(new SendEmailCommand
            {
                To = user.Email,
                Subject = "Eğitmenlik başvurunuz onaylandı 🎉",
                Body = $"Merhaba {user.FirstName},\n\nEğitmenlik başvurunuz onaylandı. Tekrar giriş yaptıktan sonra eğitmen panelinden kurs açabilirsiniz.\n\n{application.AdminNote}"
            });

            return Ok("Başvuru onaylandı.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/Reject")]
        public async Task<IActionResult> Reject(int id, ReviewTeacherApplicationDto dto)
        {
            var application = _applicationService.TGetById(id);
            if (application == null) return NotFound();
            if (application.Status != ApplicationStatus.Pending) return Conflict("Bu başvuru zaten değerlendirilmiş.");

            application.Status = ApplicationStatus.Rejected;
            application.AdminNote = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim();
            application.ReviewedAt = DateTime.Now;
            _applicationService.TUpdate(application);

            var user = await _userManager.FindByIdAsync(application.AppUserId.ToString());
            if (user?.Email != null)
            {
                await _publishEndpoint.Publish(new SendEmailCommand
                {
                    To = user.Email,
                    Subject = "Eğitmenlik başvurunuz hakkında",
                    Body = $"Merhaba {user.FirstName},\n\nEğitmenlik başvurunuz bu sefer onaylanmadı.\n\n{application.AdminNote}\n\nİleride tekrar başvurabilirsiniz."
                });
            }

            return Ok("Başvuru reddedildi.");
        }
    }
}
