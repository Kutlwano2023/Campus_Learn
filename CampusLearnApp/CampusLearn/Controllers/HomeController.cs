using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CampusLearn.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "STUDENT")]
        public IActionResult StudentPortal()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Student";
            return View();
        }

        [Authorize(Roles = "TUTOR")]
        public IActionResult TutorPortal()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Tutor";
            return View();
        }
    }
}