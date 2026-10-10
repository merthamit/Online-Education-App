using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.BlogCategoryDtos;
using OnlineEdu.WebUI.DTOs.BlogDtos;
using OnlineEdu.WebUI.DTOs.BlogCommentDtos;
using OnlineEdu.WebUI.DTOs.SubscriberDtos;
using OnlineEdu.WebUI.Helpers;
using OnlineEdu.WebUI.Models;
using System.Threading.Tasks;

namespace OnlineEdu.WebUI.Controllers
{
    public class BlogController : Controller
    {
        private readonly HttpClient _client;

        public BlogController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }
        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Subscribe(CreateSubscriberDto model)
        {
            await _client.PostAsJsonAsync("subscribers", model);
            return NoContent();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment(CreateBlogCommentDto model)
        {
            model.Name = User.FindFirst("fullName")?.Value ?? User.Identity?.Name;
            model.Email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
            await _client.PostAsJsonAsync("BlogComments", model);
            return RedirectToAction("BlogDetails", new { id = model.BlogId });
        }
                public async Task<IActionResult> BlogDetails(int id)
        {
            var values = await GetOrNullAsync<ResultBlogDto>($"blogs/{id}");
            if (values == null) return NotFound();
            var comments = await _client.GetFromJsonAsync<List<ResultBlogCommentDto>>($"BlogComments/{id}");
            ViewBag.Comments = comments;

            // Önceki (daha eski) ve sonraki (daha yeni) yazı
            var allBlogs = (await _client.GetFromJsonAsync<List<ResultBlogDto>>("blogs") ?? new List<ResultBlogDto>())
                .OrderBy(x => x.BlogDate).ThenBy(x => x.BlogId).ToList();
            var index = allBlogs.FindIndex(x => x.BlogId == id);
            if (index >= 0)
            {
                ViewBag.PrevBlog = index > 0 ? allBlogs[index - 1] : null;
                ViewBag.NextBlog = index < allBlogs.Count - 1 ? allBlogs[index + 1] : null;
            }
            return View(values);
        }

        public async Task<IActionResult> BlogsByCategory(int id, int page = 1)
        {
            var values = await _client.GetFromJsonAsync<List<ResultBlogDto>>($"blogs/GetBlogsByCategoryId/{id}") ?? new List<ResultBlogDto>();
            ViewBag.categoryName = values.Select(x => x.BlogCategory?.Name).FirstOrDefault();
            var pageItems = PaginationViewModel.Paginate(values.OrderByDescending(x => x.BlogDate).ToList(), page, 3, $"/Blog/BlogsByCategory/{id}", out var pagination);
            ViewBag.Pagination = pagination;
            return View(pageItems);
        }

        private async Task<T?> GetOrNullAsync<T>(string url) where T : class
        {
            var response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;
            var content = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(content) ? null : System.Text.Json.JsonSerializer.Deserialize<T>(content, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));
        }
    }
}
