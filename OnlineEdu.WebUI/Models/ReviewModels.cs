namespace OnlineEdu.WebUI.Models
{
    public class CourseReviewDto
    {
        public int CourseReviewId { get; set; }
        public int CourseId { get; set; }
        public int AppUserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserFullName { get; set; } = "";
        public string? UserImageUrl { get; set; }
    }

    public class ReviewSummaryDto
    {
        public int CourseId { get; set; }
        public double Average { get; set; }
        public int Count { get; set; }
    }

    public class TeacherApplicationDto
    {
        public int TeacherApplicationId { get; set; }
        public int AppUserId { get; set; }
        public string Expertise { get; set; } = "";
        public string Motivation { get; set; } = "";
        public string? PortfolioUrl { get; set; }
        public string Status { get; set; } = "Pending";
        public string? AdminNote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string UserFullName { get; set; } = "";
        public string? UserEmail { get; set; }
        public string? UserImageUrl { get; set; }
    }
}
