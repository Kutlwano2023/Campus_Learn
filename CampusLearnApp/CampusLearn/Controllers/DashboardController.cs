using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CampusLearn.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var userRole = User.IsInRole("TUTOR") ? "TUTOR" : "STUDENT";

            if (userRole == "TUTOR")
            {
                return RedirectToAction("TutorDashboard");
            }
            else
            {
                return RedirectToAction("StudentDashboard");
            }
        }

        [Authorize(Roles = "STUDENT")]
        public IActionResult StudentDashboard()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Student";
            return View();
        }

        [Authorize(Roles = "TUTOR")]
        public IActionResult TutorDashboard()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Tutor";
            return View();
        }
    }
}