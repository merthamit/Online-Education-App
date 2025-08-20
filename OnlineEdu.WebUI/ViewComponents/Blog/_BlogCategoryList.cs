using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.BlogCategoryDtos;
using OnlineEdu.WebUI.Helpers;
using OnlineEdu.WebUI.Models;

namespace OnlineEdu.WebUI.ViewComponents.Blog
{
    public class _BlogCategoryList : ViewComponent
    {
        private readonly HttpClient _client;

        public _BlogCategoryList(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categoryList = await _client.GetFromJsonAsync<List<ResultBlogCategoryDto>>("blogcategories");

            var blogCategories = (from blogCategory in categoryList select new BlogCategoryWithCountViewModel {
                BlogCategoryId = blogCategory.BlogCategoryId,
                CategoryName = blogCategory.Name,
                BlogCount = blogCategory.Blogs.Count
            }).ToList();
            return View(blogCategories);
        }
    }
}
