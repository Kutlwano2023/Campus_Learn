using CampusLearn.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System.Security.Claims;

namespace CampusLearn.Services
{
    public class UserService
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly MongoService _mongoService;

        public UserService(UserManager<Users> userManager, SignInManager<Users> signInManager, MongoService mongoService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mongoService = mongoService;
        }

        public async Task<bool> RegisterUserAsync(string fullName, string email, string password, string role = "Student")
        {
            try
            {
                var user = new Users
                {
                    FullName = fullName,
                    UserName = email,
                    Email = email,
                    Role = role,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role));
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                return false;
            }
        }

        public async Task<SignInResult> LoginUserAsync(string email, string password, bool rememberMe = false)
        {
            try
            {
                return await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return SignInResult.Failed;
            }
        }

        public async Task<Users> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<List<Users>> GetAllUsersAsync()
        {
            return await _mongoService.Users.Find(_ => true).ToListAsync();
        }
    }
}