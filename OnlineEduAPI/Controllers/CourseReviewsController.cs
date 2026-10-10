using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OnlineEdu.Business.Abstract;
using OnlineEdu.Entity.Entities;
using System.Security.Claims;
using System.Text.Json;

namespace OnlineEduAPI.Controllers
{
    public record CreateCourseReviewDto(int CourseId, int Rating, string? Comment);

    [Route("api/[controller]")]
    [ApiController]
    public class CourseReviewsController(
        IGenericService<CourseReview> _reviewService,
        ICourseRegisterService _courseRegisterService,
        IDistributedCache _cache) : ControllerBase
    {
        private const string SummaryCacheKey = "course_review_summary";

        private int CurrentUserId => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;

        private static object ToDto(CourseReview r) => new
        {
            r.CourseReviewId,
            r.CourseId,
            r.AppUserId,
            r.Rating,
            r.Comment,
            r.CreatedAt,
            UserFullName = r.AppUser != null ? $"{r.AppUser.FirstName} {r.AppUser.LastName}" : "Öğrenci",
            UserImageUrl = r.AppUser?.ImageUrl
        };

        // Kursun yorumları (en yeni önce)
        [HttpGet("Course/{courseId}")]
        public IActionResult GetByCourse(int courseId)
        {
            var reviews = _reviewService.TGetFilteredList(x => x.CourseId == courseId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(ToDto)
                .ToList();
            return Ok(reviews);
        }

        // Tüm kursların ortalama puanı ve puan sayısı (Redis'te önbelleklenir)
        [HttpGet("Summary")]
        public async Task<IActionResult> Summary()
        {
            var cached = await _cache.GetStringAsync(SummaryCacheKey);
            if (!string.IsNullOrEmpty(cached))
            {
                return Content(cached, "application/json");
            }

            var summary = _reviewService.TGetList()
                .GroupBy(x => x.CourseId)
                .Select(g => new { CourseId = g.Key, Average = Math.Round(g.Average(x => x.Rating), 1), Count = g.Count() })
                .ToList();

            var json = JsonSerializer.Serialize(summary, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            await _cache.SetStringAsync(SummaryCacheKey, json, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
            return Content(json, "application/json");
        }

        // Giriş yapan öğrencinin bu kursa verdiği puan (yoksa 204)
        [Authorize]
        [HttpGet("My/{courseId}")]
        public IActionResult MyReview(int courseId)
        {
            var review = _reviewService.TGetByFilter(x => x.CourseId == courseId && x.AppUserId == CurrentUserId);
            return review == null ? NoContent() : Ok(ToDto(review));
        }

        // Puan ver veya mevcut puanını güncelle (sadece kursa kayıtlı öğrenciler)
        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> Upsert(CreateCourseReviewDto dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                return BadRequest("Puan 1 ile 5 arasında olmalıdır.");
            if (dto.Comment?.Length > 1000)
                return BadRequest("Yorum en fazla 1000 karakter olabilir.");

            var userId = CurrentUserId;
            var enrolled = _courseRegisterService.TFilteredCount(x => x.CourseId == dto.CourseId && x.AppUserId == userId) > 0;
            if (!enrolled)
                return StatusCode(StatusCodes.Status403Forbidden, "Puan verebilmek için bu kursa kayıtlı olmalısınız.");

            var comment = string.IsNullOrWhiteSpace(dto.Comment) ? null : dto.Comment.Trim();
            var existing = _reviewService.TGetByFilter(x => x.CourseId == dto.CourseId && x.AppUserId == userId);
            if (existing == null)
            {
                _reviewService.TCreate(new CourseReview
                {
                    CourseId = dto.CourseId,
                    AppUserId = userId,
                    Rating = dto.Rating,
                    Comment = comment,
                    CreatedAt = DateTime.Now
                });
            }
            else
            {
                existing.Rating = dto.Rating;
                existing.Comment = comment;
                existing.CreatedAt = DateTime.Now;
                _reviewService.TUpdate(existing);
            }

            await _cache.RemoveAsync(SummaryCacheKey);
            return Ok(existing == null ? "Değerlendirmeniz kaydedildi." : "Değerlendirmeniz güncellendi.");
        }

        // Kendi yorumunu (veya admin herhangi bir yorumu) siler
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var review = _reviewService.TGetById(id);
            if (review == null) return NotFound();
            if (review.AppUserId != CurrentUserId && !User.IsInRole("Admin")) return Forbid();

            _reviewService.TDelete(id);
            await _cache.RemoveAsync(SummaryCacheKey);
            return Ok("Değerlendirme silindi.");
        }
    }
}
