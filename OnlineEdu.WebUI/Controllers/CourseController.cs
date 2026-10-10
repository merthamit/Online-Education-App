using Microsoft.AspNetCore.Authorization;
using OnlineEdu.WebUI.DTOs.UserDtos;
using OnlineEdu.WebUI.DTOs.CourseCategoryDtos;
using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.DTOs.CourseDtos;
using OnlineEdu.WebUI.DTOs.CourseRegisterDtos;
using System.Security.Claims;
using OnlineEdu.WebUI.Helpers;
using OnlineEdu.WebUI.Models;
using System.Threading.Tasks;

namespace OnlineEdu.WebUI.Controllers
{
    public class CourseController : Controller
    {
        private readonly HttpClient _client;

        public CourseController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("EduClient");
        }
        private const int PageSize = 6;

        public async Task<IActionResult> Index(CourseFilterViewModel filter, int page = 1)
        {
            var courses = await _client.GetFromJsonAsync<List<ResultCourseDto>>("courses/GetActiveCourses") ?? new List<ResultCourseDto>();
            filter.Categories = (await _client.GetFromJsonAsync<List<ResultCourseCategoryDto>>("coursecategories/GetActiveCategories") ?? new List<ResultCourseCategoryDto>())
                .OrderBy(x => x.CategoryName).ToList();

            await _client.AttachRatingsAsync(courses);
            IEnumerable<ResultCourseDto> query = courses;
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(x => x.CourseName.ContainsTr(filter.Search) || (x.Category?.CategoryName).ContainsTr(filter.Search)
                                      || (x.AppUser != null && (x.AppUser.FirstName + " " + x.AppUser.LastName).ContainsTr(filter.Search)));
            }
            if (filter.Category.HasValue)
            {
                query = query.Where(x => x.CourseCategoryId == filter.Category.Value);
            }
            if (filter.Teacher.HasValue)
            {
                query = query.Where(x => x.AppUserId == filter.Teacher.Value);
                var teachers = await _client.GetFromJsonAsync<List<ResultUserDto>>("users/TeacherList") ?? new List<ResultUserDto>();
                ViewBag.Teacher = teachers.FirstOrDefault(x => x.Id == filter.Teacher.Value);

                // Eğitmen kendi sayfasını görüntülüyorsa "Kurs Aç" ve onay bekleyen kurs sayısını göster
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (User.IsInRole("Teacher") && currentUserId == filter.Teacher.Value.ToString())
                {
                    ViewBag.IsOwnPage = true;
                    var myCourses = await _client.GetFromJsonAsync<List<ResultCourseDto>>($"courses/GetCoursesByTeacherId/{currentUserId}") ?? new List<ResultCourseDto>();
                    ViewBag.PendingCount = myCourses.Count(x => !x.IsShown);
                }
            }
            if (filter.Rating.HasValue)
            {
                query = query.Where(x => x.ReviewCount > 0 && x.AverageRating >= filter.Rating.Value);
            }
            query = filter.Price switch
            {
                "free" => query.Where(x => x.Price <= 0),
                "paid" => query.Where(x => x.Price > 0),
                _ => query
            };
            query = filter.Sort switch
            {
                "rating" => query.OrderByDescending(x => x.AverageRating).ThenByDescending(x => x.ReviewCount),
                "popular" => query.OrderByDescending(x => x.ReviewCount).ThenByDescending(x => x.AverageRating),
                "price-asc" => query.OrderBy(x => x.Price),
                "price-desc" => query.OrderByDescending(x => x.Price),
                "name" => query.OrderBy(x => x.CourseName, StringComparer.Create(new System.Globalization.CultureInfo("tr-TR"), true)),
                _ => query.OrderByDescending(x => x.CourseId)
            };

            var filtered = query.ToList();
            filter.TotalCount = filtered.Count;

            var categoryName = filter.Category.HasValue ? filter.Categories.FirstOrDefault(x => x.CourseCategoryId == filter.Category)?.CategoryName : null;
            var teacher = ViewBag.Teacher as ResultUserDto;
            ViewBag.Heading = !string.IsNullOrWhiteSpace(filter.Search) ? $"\"{filter.Search}\" için sonuçlar"
                            : teacher != null ? $"{teacher.FirstName} {teacher.LastName}"
                            : categoryName != null ? $"{categoryName} Kursları" : "Tüm Kurslar";
            ViewBag.Filter = filter;

            var pageItems = PaginationViewModel.Paginate(filtered, page, PageSize, "/Course/Index", out var pagination);
            pagination.Query = filter.ToQuery();
            ViewBag.Pagination = pagination;
            return View(pageItems);
        }

        // Eski kategori linkleri filtreli listeye yönlenir
        public IActionResult GetCoursesByCategoryId(int id)
        {
            return RedirectToAction(nameof(Index), new { category = id });
        }

            public async Task<IActionResult> CourseDetails(int id)
        {
            var response = await _client.GetAsync($"courses/{id}");
            var content = response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
            if (string.IsNullOrWhiteSpace(content)) return NotFound();
            var course = System.Text.Json.JsonSerializer.Deserialize<ResultCourseDto>(content, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web));
            if (course == null) return NotFound();

            ViewBag.IsEnrolled = false;
            if (User.IsInRole("Student"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var myCourses = await _client.GetFromJsonAsync<List<ResultCourseRegisterDto>>($"courseRegisters/GetMyCourses/{userId}");
                ViewBag.IsEnrolled = myCourses?.Any(x => x.CourseId == id) ?? false;

                var myReviewResponse = await _client.GetAsync($"CourseReviews/My/{id}");
                if (myReviewResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ViewBag.MyReview = await myReviewResponse.Content.ReadFromJsonAsync<CourseReviewDto>();
                }
            }

            var reviews = await _client.GetFromJsonAsync<List<CourseReviewDto>>($"CourseReviews/Course/{id}") ?? new List<CourseReviewDto>();
            ViewBag.Reviews = reviews;
            course.ReviewCount = reviews.Count;
            course.AverageRating = reviews.Any() ? Math.Round(reviews.Average(x => x.Rating), 1) : 0;
            return View(course);
        }

        // Öğrenci kursa puan verir / puanını günceller
        [Authorize(Roles = "Student")]
        [HttpPost]
        public async Task<IActionResult> Review(int courseId, int rating, string? comment)
        {
            var response = await _client.PostAsJsonAsync("CourseReviews", new { courseId, rating, comment });
            if (response.IsSuccessStatusCode)
            {
                TempData["ReviewMessage"] = await response.Content.ReadAsStringAsync();
                TempData["ReviewOk"] = true;
            }
            else
            {
                var text = await response.Content.ReadAsStringAsync();
                TempData["ReviewMessage"] = text.Length < 200 && !text.TrimStart().StartsWith("{") ? text : "Değerlendirme kaydedilemedi, lütfen tekrar deneyin.";
                TempData["ReviewOk"] = false;
            }
            return Redirect($"/Course/CourseDetails/{courseId}#degerlendirmeler");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteReview(int id, int courseId)
        {
            var response = await _client.DeleteAsync($"CourseReviews/{id}");
            TempData["ReviewMessage"] = response.IsSuccessStatusCode ? "Değerlendirmeniz silindi." : "Değerlendirme silinemedi.";
            TempData["ReviewOk"] = response.IsSuccessStatusCode;
            return Redirect($"/Course/CourseDetails/{courseId}#degerlendirmeler");
        }
    }
}