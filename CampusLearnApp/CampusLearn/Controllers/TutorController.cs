using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusLearn.Controllers
{
    [Authorize(Roles = "Tutor")]
    public class TutorController : Controller
    {
        public IActionResult Dashboard()
        {
            // Get current user info
            var userName = User.Identity.Name;
            ViewBag.UserName = userName;
            ViewBag.Role = "Tutor";

            return View();
        }
    }
}