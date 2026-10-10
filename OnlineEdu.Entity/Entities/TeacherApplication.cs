namespace OnlineEdu.Entity.Entities
{
    public enum ApplicationStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    // Öğrencinin eğitmen olmak için yaptığı başvuru
    public class TeacherApplication
    {
        public int TeacherApplicationId { get; set; }
        public int AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }
        public string Expertise { get; set; } = "";
        public string Motivation { get; set; } = "";
        public string? PortfolioUrl { get; set; }
        public ApplicationStatus Status { get; set; }
        public string? AdminNote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}
