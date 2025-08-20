using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineEdu.Business.Abstract;
using OnlineEdu.DataAccess.Context;
using OnlineEdu.DTO.DTOs.UserDtos;
using OnlineEdu.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineEdu.Business.Concrete
{
    public class UserService(UserManager<AppUser> _userManager, OnlineEduContext _context, SignInManager<AppUser> _signInManager, RoleManager<AppRole> _roleManager, IMapper _mapper) : IUserService
    {
        public async Task<bool> AssignRoleAsync(List<AssignRoleDto> assignRoleDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateRoleAsync(UserRoleDto userRoleDto)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> CreateUserAsync(RegisterDto userRegisterDto)
        {
            var user = new AppUser
            {
                FirstName = userRegisterDto.FirstName,
                LastName = userRegisterDto.LastName,
                UserName = userRegisterDto.UserName,
                Email = userRegisterDto.Email
            };
            if (userRegisterDto.Password != userRegisterDto.Password)
            {
                return new IdentityResult();
            }
            var result = await _userManager.CreateAsync(user, userRegisterDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student");
            }
            return result;

        }

        public async Task<List<ResultUserDto>> Get4Teachers()
        {
            var users = await _userManager.Users.Include(x => x.TeacherSocials).ToListAsync();
            var teachers = users.Where(user => _userManager.IsInRoleAsync(user, "Teacher").Result).OrderByDescending(x => x.Id).Take(4).ToList();
            var mapper = _mapper.Map<List<ResultUserDto>>(teachers);
            return mapper;
        }

        public async Task<List<ResultUserDto>> GetAllTeachers()
        {
            var teachersBasic = await _userManager.GetUsersInRoleAsync("Teacher");

            var teacherIds = teachersBasic.Select(t => t.Id).ToList();

            var teachersWithSocials = await _context.Users
                .Include(u => u.TeacherSocials)
                .Where(u => teacherIds.Contains(u.Id))
                .ToListAsync();

            return _mapper.Map<List<ResultUserDto>>(teachersWithSocials);
        }

        public async Task<List<AppUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<string> LoginAsync(LoginDto userLoginDto)
        {
            var user = await _userManager.FindByEmailAsync(userLoginDto.Email);
            if (user == null)
            {
                return null;
            }

            var result = await _signInManager.PasswordSignInAsync(user, userLoginDto.Password, false, false);
            if (!result.Succeeded)
            {
                return null;
            }

            var userRoleList = new List<string> { "Admin", "Teacher", "Student" };

            foreach (var item in userRoleList)
            {
                var isInRole = await _userManager.IsInRoleAsync(user, item);
                if (isInRole)
                {
                    return item;
                }
            }

            return null;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
