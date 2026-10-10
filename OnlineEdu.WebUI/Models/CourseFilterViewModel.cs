using OnlineEdu.WebUI.DTOs.CourseCategoryDtos;

namespace OnlineEdu.WebUI.Models
{
    public class CourseFilterViewModel
    {
        public string? Search { get; set; }
        public int? Category { get; set; }
        public int? Teacher { get; set; }
        public string? Price { get; set; }
        public double? Rating { get; set; }  // en az puan (ör. 4)   // "free" | "paid"
        public string? Sort { get; set; }    // "newest" | "rating" | "popular" | "price-asc" | "price-desc" | "name"

        public int TotalCount { get; set; }
        public List<ResultCourseCategoryDto> Categories { get; set; } = new();

        public bool HasFilter => !string.IsNullOrWhiteSpace(Search) || Category.HasValue || Teacher.HasValue || Rating.HasValue || !string.IsNullOrWhiteSpace(Price);

        public bool HasFilterExceptTeacher => !string.IsNullOrWhiteSpace(Search) || Category.HasValue || Rating.HasValue || !string.IsNullOrWhiteSpace(Price);

        public Dictionary<string, string?> ToQuery() => new()
        {
            ["search"] = Search,
            ["category"] = Category?.ToString(),
            ["teacher"] = Teacher?.ToString(),
            ["price"] = Price,
            ["rating"] = Rating?.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["sort"] = Sort
        };
    }
}
