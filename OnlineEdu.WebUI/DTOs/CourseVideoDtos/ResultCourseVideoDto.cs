using OnlineEdu.WebUI.DTOs.CourseDtos;

namespace OnlineEdu.WebUI.DTOs.CourseVideoDtos
{
    public class ResultCourseVideoDto
    {
        public int CourseVideoId { get; set; }
        public int VideoNumber { get; set; }
        public int CourseId { get; set; }
        public virtual ResultCourseDto Course { get; set; }
        public string VideoUrl { get; set; }
    }
}
