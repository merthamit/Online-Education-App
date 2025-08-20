using AutoMapper;
using OnlineEdu.DTO.DTOs.CourseVideoDtos;
using OnlineEdu.Entity.Entities;

namespace OnlineEduAPI.Mapping
{
    public class CourseVideoMapping : Profile
    {
        public CourseVideoMapping() {
            CreateMap<CreateCourseVideoDto, CourseVideo>().ReverseMap();
            CreateMap<UpdateCourseVideo, CourseVideo>().ReverseMap();
            CreateMap<ResultCourseVideo, CourseVideo>().ReverseMap();
        }
    }
}
