using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.CourseDtos;
using OnlineEdu.WebUI.Helpers;

namespace OnlineEdu.WebUI.ViewComponents.Home
{
    public class _HomeCourseComponent : ViewComponent
    {
        private readonly HttpClient _client;

        public _HomeCourseComponent(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }
        public async Task <IViewComponentResult> InvokeAsync()
        {
            var values = await _client.GetFromJsonAsync<List<ResultCourseDto>>("courses/GetActiveCourses") ?? new List<ResultCourseDto>();
            await _client.AttachRatingsAsync(values);
            return View(values.OrderByDescending(x => x.AverageRating).ThenByDescending(x => x.CourseId).Take(6).ToList());
        }
    }
}
