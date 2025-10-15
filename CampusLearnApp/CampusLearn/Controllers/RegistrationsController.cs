using Microsoft.AspNetCore.Mvc;
using CampusLearn.Data;
using CampusLearn.Models;
using System.Threading.Tasks;

namespace CampusLearn.Controllers
{
    public class RegistrationsController : Controller
    {
        private readonly AppDbContext _context;

        public RegistrationsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Registration registration)
        {
            if (ModelState.IsValid)
            {
                _context.Registrations.Add(registration);
                await _context.SaveChangesAsync();
                return RedirectToAction("Thanks");
            }

            return View(registration);
        }

        public IActionResult Thanks()
        {
            return View();
        }
    }
}
