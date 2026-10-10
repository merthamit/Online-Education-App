using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.BlogDtos;
using OnlineEdu.WebUI.Helpers;
using OnlineEdu.WebUI.Models;
using System.Threading.Tasks;

namespace OnlineEdu.WebUI.ViewComponents.Blog
{
    public class _BlogAllBlogs : ViewComponent
    {
        private readonly HttpClient _client;

        public _BlogAllBlogs(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }
        private const int PageSize = 3;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _client.GetFromJsonAsync<List<ResultBlogDto>>("blogs") ?? new List<ResultBlogDto>();
            int.TryParse(HttpContext.Request.Query["page"], out var page);
            var search = HttpContext.Request.Query["search"].ToString();
            if (!string.IsNullOrWhiteSpace(search))
            {
                values = values.Where(x => x.Title.ContainsTr(search) || x.Content.ContainsTr(search) || (x.BlogCategory?.Name).ContainsTr(search)).ToList();
                ViewBag.Search = search;
                ViewBag.ResultCount = values.Count;
            }
            var pageItems = PaginationViewModel.Paginate(values.OrderByDescending(x => x.BlogDate).ToList(), page, PageSize, "/Blog/Index", out var pagination, search);
            ViewBag.Pagination = pagination;
            return View(pageItems);
        }
    }
}
