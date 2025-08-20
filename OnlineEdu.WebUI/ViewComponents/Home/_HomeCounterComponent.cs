using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.AboutDtos;
using OnlineEdu.WebUI.Helpers;

namespace OnlineEdu.WebUI.ViewComponents.Home
{
    public class _HomeCounterComponent : ViewComponent
    {
        private readonly HttpClient _client;

        public _HomeCounterComponent(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewBag.blogCount = await _client.GetFromJsonAsync<int>("blogs/GetBlogCount");
            ViewBag.courseCount = await _client.GetFromJsonAsync<int>("courses/GetCourseCount");
            ViewBag.testimonialCount = await _client.GetFromJsonAsync<int>("testimonials/GetTestimonialCount");
            ViewBag.courseCategoryCount = await _client.GetFromJsonAsync<int>("coursecategories/GetCourseCategoryCount");
            return View();
        }
    }
}
