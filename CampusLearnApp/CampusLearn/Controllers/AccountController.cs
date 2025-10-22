using CampusLearn.Models;
using CampusLearn.Services;
using CampusLearn.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CampusLearn.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly MongoService _mongoService;

        public AccountController(
            UserManager<Users> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<Users> signInManager,
            MongoService mongoService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mongoService = mongoService;
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
            ModelState.Remove("ReturnUrl");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var userRole = roles.FirstOrDefault() ?? user.Role;

                    if (!string.IsNullOrEmpty(userRole) && userRole.Equals("TUTOR", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("TutorPortal", "Portal");
                    }
                    else
                    {
                        return RedirectToAction("StudentPortal", "Portal");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during login.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            Console.WriteLine("=== REGISTRATION STARTED ===");
            Console.WriteLine($"Email: {model.Email}, FullName: {model.FullName}, Role: {model.Role}");

            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState is VALID");

                try
                {
                    // Check if user already exists
                    Console.WriteLine("Checking if user exists...");
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null)
                    {
                        Console.WriteLine("User already exists!");
                        ModelState.AddModelError("", "User with this email already exists.");
                        return View(model);
                    }

                    Console.WriteLine("Creating new user object...");
                    var user = new Users
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FullName = model.FullName,
                        Role = model.Role?.ToUpper(),
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    Console.WriteLine("Calling UserManager.CreateAsync...");
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        Console.WriteLine("User created SUCCESSFULLY!");
                        Console.WriteLine($"User ID: {user.Id}");

                        // Add user to role
                        if (!string.IsNullOrEmpty(model.Role))
                        {
                            var roleName = model.Role.ToUpper();
                            Console.WriteLine($"Adding user to role: {roleName}");

                            // Ensure role exists
                            if (!await _roleManager.RoleExistsAsync(roleName))
                            {
                                Console.WriteLine($"Creating role: {roleName}");
                                await _roleManager.CreateAsync(new ApplicationRole(roleName));
                            }

                            var roleResult = await _userManager.AddToRoleAsync(user, roleName);
                            if (roleResult.Succeeded)
                            {
                                Console.WriteLine("User added to role SUCCESSFULLY");
                            }
                            else
                            {
                                Console.WriteLine("FAILED to add user to role:");
                                foreach (var error in roleResult.Errors)
                                {
                                    Console.WriteLine($"Role Error: {error.Description}");
                                }
                            }
                        }

                        Console.WriteLine("Setting success messages and redirecting to Login...");
                        TempData["RegistrationSuccess"] = true;
                        TempData["SuccessMessage"] = "Thank you for registering! Please login with your credentials.";

                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        Console.WriteLine("User creation FAILED:");
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error: {error.Code} - {error.Description}");
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"EXCEPTION during registration:");
                    Console.WriteLine($"Message: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    ModelState.AddModelError("", $"An error occurred during registration: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("ModelState is INVALID:");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    if (errors.Count > 0)
                    {
                        Console.WriteLine($"Field: {key}");
                        foreach (var error in errors)
                        {
                            Console.WriteLine($"  - {error.ErrorMessage}");
                        }
                    }
                }
            }

            Console.WriteLine("Returning to Register view with errors");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}