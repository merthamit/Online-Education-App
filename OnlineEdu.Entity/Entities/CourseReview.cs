using System.Text.Json.Serialization;

namespace OnlineEdu.Entity.Entities
{
    // Öğrencinin kayıtlı olduğu kursa verdiği puan ve yorum (her öğrenci bir kursa bir kez puan verir)
    public class CourseReview
    {
        public int CourseReviewId { get; set; }
        public int CourseId { get; set; }
        [JsonIgnore] public virtual Course? Course { get; set; }
        public int AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
