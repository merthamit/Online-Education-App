using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DTO.DTOs.UserDtos;
using OnlineEdu.Entity.Entities;
using OnlineEdu.WebUI.Models;
using System.Security.Claims;

namespace OnlineEduAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleAssignsController(IUserService _userService, UserManager<AppUser> _userManager,
        RoleManager<AppRole> _roleManager) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var values = await _userService.GetAllUsersAsync();
            var userList = new List<UserListDto>();

            foreach (var user in values)
            {
                var userRole = await _userManager.GetRolesAsync(user);

                userList.Add(new UserListDto
                {
                    Id = user.Id,
                    NameSurname = user.FirstName + " " + user.LastName,
                    UserName = user.UserName,
                    Roles = userRole.ToList()
                });
            }

            return Ok(userList);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetUserForRoleAssign(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            var roles = await _roleManager.Roles.ToListAsync();

            var userRoles = await _userManager.GetRolesAsync(user);

            List<AssignRoleDto> assignRoleList = new List<AssignRoleDto>();

            foreach (var role in roles)
            {
                var assignRole = new AssignRoleDto();
                assignRole.RoleId = role.Id;
                assignRole.UserId = user.Id;
                assignRole.RoleName = role.Name;
                assignRole.RoleExist = userRoles.Contains(role.Name);

                assignRoleList.Add(assignRole);
            }
            return Ok(assignRoleList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(List<AssignRoleDto> assignRoleList)
        {
            int userId = assignRoleList.Select(x => x.UserId).FirstOrDefault();

            var user = await _userService.GetUserByIdAsync(userId);

            foreach (var item in assignRoleList)
            {
                if (item.RoleExist)
                {
                    await _userManager.AddToRoleAsync(user, item.RoleName);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, item.RoleName);
                }
            }

            return Ok("Rol atama başarılı.");
        }

    }
}
