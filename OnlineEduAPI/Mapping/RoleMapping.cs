using AutoMapper;
using OnlineEdu.DTO.DTOs.RoleDtos;
using OnlineEdu.Entity.Entities;

namespace OnlineEduAPI.Mapping
{
    public class RoleMapping : Profile
    {
        public RoleMapping()
        {
            CreateMap<CreateRoleDto, AppRole>().ReverseMap();
        }
    }
}
