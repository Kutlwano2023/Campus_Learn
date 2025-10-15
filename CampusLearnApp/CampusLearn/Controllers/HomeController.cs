using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusLearn.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult StudentPortal()
        {
            return View();
        }

        [Authorize]
        public IActionResult TutorPortal()
        {
            return View();
        }
    }
}