using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusLearn.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        public IActionResult Dashboard()
        {
            // Get current user info
            var userName = User.Identity.Name;
            ViewBag.UserName = userName;
            ViewBag.Role = "Student";

            return View();
        }
    }
}