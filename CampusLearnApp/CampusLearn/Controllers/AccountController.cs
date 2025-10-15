using CampusLearn.Data;
using CampusLearn.Models;
using CampusLearn.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CampusLearn.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Users> _signInManager;

        // Constructor includes SignInManager now
        public AccountController(
            AppDbContext context,
            UserManager<Users> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<Users> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Remove ReturnUrl from validation
            ModelState.Remove("ReturnUrl");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                Console.WriteLine("Checking if user exists...");
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    Console.WriteLine("User not found in database");
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }

                Console.WriteLine($"User found: {user.UserName}");
                Console.WriteLine($"User ID: {user.Id}");
                Console.WriteLine($"User FullName: {user.FullName}");
                Console.WriteLine($"User Role: {user.Role}");

                Console.WriteLine("Attempting to sign in user...");

                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                Console.WriteLine($"SignIn result: {result.Succeeded}");

                if (result.Succeeded)
                {
                    Console.WriteLine("Login successful!");

                    // Get user roles from Identity
                    var roles = await _userManager.GetRolesAsync(user);
                    Console.WriteLine($"Identity Roles: {string.Join(", ", roles)}");

                    // Use Identity roles first, fall back to user.Role from database
                    var userRole = roles.FirstOrDefault() ?? user.Role;
                    Console.WriteLine($"Final role determined: {userRole}");

                    // Redirect based on role - FIXED LOGIC
                    if (!string.IsNullOrEmpty(userRole) && userRole.Equals("Tutor", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Redirecting to TUTOR portal");
                        return RedirectToAction("TutorPortal", "Home");
                    }
                    else
                    {
                        Console.WriteLine("Redirecting to STUDENT portal");
                        return RedirectToAction("StudentPortal", "Home");
                    }
                }
                else
                {
                    Console.WriteLine("PasswordSignInAsync failed");
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION during login: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", "An error occurred during login.");
            }

            return View(model);
        }
        private IActionResult RedirectToRolePortal(string role)
        {
            Console.WriteLine($"Redirecting based on role: {role}");

            var redirectResult = role?.ToLower() switch
            {
                "student" => RedirectToAction("StudentPortal", "Home"),
                "tutor" => RedirectToAction("TutorPortal", "Home"),
                _ => RedirectToAction("Index", "Home")
            };

            Console.WriteLine($"Redirecting to: {redirectResult}");
            return redirectResult;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Registration model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new Users
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FullName = model.FullName,
                        Role = model.Role
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        // Save registration record
                        var registration = new Registration
                        {
                            FullName = model.FullName,
                            Email = model.Email,
                            Password = "REGISTERED",
                            Role = model.Role
                        };

                        _context.Registrations.Add(registration);
                        await _context.SaveChangesAsync();

                        await _signInManager.SignInAsync(user, isPersistent: false);

                        // Pass success message to view
                        TempData["RegistrationSuccess"] = true;

                        // Redirect based on role
                        if (model.Role?.ToLower() == "tutor")
                        {
                            return RedirectToAction("TutorPortal", "Home");
                        }
                        else
                        {
                            return RedirectToAction("StudentPortal", "Home");
                        }
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred during registration.");
                }
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            Console.WriteLine("Logout called");
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
