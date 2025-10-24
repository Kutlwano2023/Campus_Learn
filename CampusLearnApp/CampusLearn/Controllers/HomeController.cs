using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CampusLearn.Models;

namespace CampusLearn.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<Users> _userManager;

        public HomeController(UserManager<Users> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // For authenticated users, redirect to the Portal dashboard action
                return RedirectToAction("Dashboard", "Portal");
            }
            else
            {
                // For non-authenticated users, show the landing page
                return View();
            }
        }

        [Authorize(Roles = "STUDENT,Student")]
        public IActionResult StudentPortal()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Student";
            return View();
        }

        [Authorize(Roles = "TUTOR,Tutor")]
        public IActionResult TutorPortal()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Tutor";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}