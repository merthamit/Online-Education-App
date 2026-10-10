using OnlineEdu.WebUI.DTOs.CourseDtos;
using OnlineEdu.WebUI.Models;

namespace OnlineEdu.WebUI.Helpers
{
    public static class RatingHelper
    {
        // Kurs listesine ortalama puan ve puan sayısını ekler (özet API'de Redis'ten gelir)
        public static async Task AttachRatingsAsync(this HttpClient client, IEnumerable<ResultCourseDto> courses)
        {
            List<ReviewSummaryDto>? summary;
            try
            {
                summary = await client.GetFromJsonAsync<List<ReviewSummaryDto>>("CourseReviews/Summary");
            }
            catch (HttpRequestException)
            {
                return; // Puanlar gelmezse kurslar yine listelensin
            }
            var map = (summary ?? new()).ToDictionary(x => x.CourseId);
            foreach (var course in courses)
            {
                if (map.TryGetValue(course.CourseId, out var s))
                {
                    course.AverageRating = s.Average;
                    course.ReviewCount = s.Count;
                }
            }
        }
    }
}
