using Microsoft.AspNetCore.Mvc;

namespace OnlineEdu.WebUI.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    public class TeacherLayoutController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("/Teacher/MyCourse/Index");
        }
    }
}
