using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusLearn.Controllers
{
    [Authorize(Roles = "TUTOR")]
    public class TutorController : Controller
    {
        public IActionResult Dashboard()
        {
            var userName = User.Identity.Name;
            ViewBag.UserName = userName;
            ViewBag.Role = "Tutor";

            return View();
        }
    }
}