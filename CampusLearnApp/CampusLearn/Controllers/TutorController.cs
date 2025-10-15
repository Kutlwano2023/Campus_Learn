using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusLearn.Controllers
{
    [Authorize(Roles = "tutor")]
    public class TutorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
