using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusLearn.Controllers
{
    [Authorize(Roles = "STUDENT")]
    public class StudentController : Controller
    {
        public IActionResult Dashboard()
        {
            var userName = User.Identity.Name;
            ViewBag.UserName = userName;
            ViewBag.Role = "Student";

            return View();
        }
    }
}