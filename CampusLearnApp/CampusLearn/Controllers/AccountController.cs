using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CampusLearn.Models;

namespace CampusLearn.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public  AccountController(UserManager<Users>userManger,SignInManager<Users> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManger;
            _signInManager = signInManager;
            _roleManager= roleManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        //Register
        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password, string role)
        {
            if (ModelState.IsValid)
            {
                // Check if role exists — if not, create it
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

                var user = new Users
                {
                    Fullname = fullName,
                    UserName = email,
                    Email = email,
                    Role = role
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Dashboard");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View("Login");
        }

        // 🔹 LOGIN
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user.Role == "Tutor")
                    return RedirectToAction("TutorDashboard", "Dashboard");
                else
                    return RedirectToAction("StudentDashboard", "Dashboard");
            }

            ModelState.AddModelError("", "Invalid login attempt");
            return View();
        }

        // 🔹 LOGOUT
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

    }
}
