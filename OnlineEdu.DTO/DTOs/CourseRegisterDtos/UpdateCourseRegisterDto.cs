using OnlineEdu.Entity.Entities;

namespace OnlineEdu.DTO.DTOs.CourseRegisterDtos
{
    public class UpdateCourseRegisterDto
    {
        public int CourseRegisterId { get; set; }
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public int CourseId { get; set; }
        public ResultCourseRegisterDto Course { get; set; }
    }
}
