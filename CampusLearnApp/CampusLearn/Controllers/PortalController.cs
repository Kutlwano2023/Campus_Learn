using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CampusLearn.Controllers
{
    [Authorize]
    public class PortalController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";
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

        public IActionResult ScheduleSession()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";
            return View();
        }

        public IActionResult Resources()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";
            return View();
        }

        public IActionResult QuizPortal()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";
            return View();
        }

        public IActionResult Tests()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";
            return View();
        }

        public IActionResult Assignments()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";
            return View();
        }

        public IActionResult Forum()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";
            return View();
        }
    }
}