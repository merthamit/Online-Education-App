using Microsoft.AspNetCore.Mvc;
using OnlineEdu.WebUI.Services.UserServices;
using System.Threading.Tasks;

namespace OnlineEdu.WebUI.Controllers
{
    public class TeacherController(IUserService _userService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var values = await _userService.GetAllTeachers();
            return View(values);
        }
    }
}
