using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.Business.Abstract;
using OnlineEdu.Entity.Entities;
using System.Security.Claims;

namespace OnlineEduAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogCommentsController : ControllerBase
    {
        private readonly IBlogCommentService _service;
        public BlogCommentsController(IBlogCommentService service) { _service = service; }

        [HttpGet("{blogId}")]
        public IActionResult GetComments(int blogId)
        {
            var comments = _service.TGetFilteredList(x => x.BlogId == blogId).OrderByDescending(x => x.CommentDate).ToList();
            return Ok(comments);
        }

        // Sadece üyeler yorum yapabilir; ad ve e-posta token'dan alınır
        [Authorize]
        [HttpPost]
        public IActionResult AddComment(BlogComment comment)
        {
            if (string.IsNullOrWhiteSpace(comment.Content))
            {
                return BadRequest("Yorum boş olamaz.");
            }

            comment.Name = User.FindFirst("fullName")?.Value ?? User.Identity?.Name ?? comment.Name;
            comment.Email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value ?? comment.Email;
            comment.CommentDate = System.DateTime.Now;
            _service.TCreate(comment);
            return Ok(comment);
        }
    }
}
