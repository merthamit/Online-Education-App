using AutoMapper;
using OnlineEdu.DTO.DTOs.CourseRegisterDtos;
using OnlineEdu.Entity.Entities;

namespace OnlineEduAPI.Mapping
{
    public class CourseRegisterMapping : Profile
    {
        public CourseRegisterMapping() {
            CreateMap<CreateCourseRegisterDto, CourseRegister>().ReverseMap();
            CreateMap<UpdateCourseRegisterDto, CourseRegister>().ReverseMap();
            CreateMap<ResultCourseRegisterDto, CourseRegister>().ReverseMap();
        }
    }
}
