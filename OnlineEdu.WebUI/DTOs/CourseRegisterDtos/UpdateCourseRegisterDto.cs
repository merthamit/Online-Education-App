using OnlineEdu.WebUI.DTOs.UserDtos;

namespace OnlineEdu.WebUI.DTOs.CourseRegisterDtos
{
    public class UpdateCourseRegisterDto
    {
        public int CourseRegisterId { get; set; }
        public int AppUserId { get; set; }
        public ResultUserDto AppUser { get; set; }
        public int CourseId { get; set; }
        public ResultCourseRegisterDto Course { get; set; }
    }
}
