using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CampusLearn.Controllers
{
    [Authorize(Roles = "TUTOR")]
    public class TutorController : Controller
    {
        private readonly ILogger<TutorController> _logger;

        public TutorController(ILogger<TutorController> logger)
        {
            _logger = logger;
        }

        public IActionResult TutorPortal()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Tutor";
            _logger.LogInformation("Tutor {UserName} accessed tutor portal", User.Identity.Name);
            return View();
        }

        public IActionResult TutorDashboard()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Tutor";
            return View();
        }

        public IActionResult MyStudents()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Tutor";
            return View();
        }

        public IActionResult SessionManagement()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Tutor";
            return View();
        }

        public IActionResult AssignmentManagement()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Tutor";
            return View();
        }

        public IActionResult GradeManagement()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Tutor";
            return View();
        }

        public IActionResult ResourceManagement()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Tutor";
            return View();
        }

        public IActionResult ProgressTracking()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Tutor";
            return View();
        }

        public IActionResult TutorAnalytics()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Tutor";
            return View();
        }

        public IActionResult TutorSettings()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Tutor";
            return View();
        }
    }
}