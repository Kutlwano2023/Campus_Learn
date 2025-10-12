using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CampusLearn.Controllers
{
    public class DashboardController : Controller
    {
        //Default page

        
        public IActionResult Index()
        {
            return View("StudentDasboard");
        }
        [Authorize(Roles ="Student")]
        public IActionResult StudentDashboard() {
            return View();
        }

        [Authorize(Roles ="Tutor")]
        public IActionResult TutorDashboard()
        {
            return View();
        }
    }
}
