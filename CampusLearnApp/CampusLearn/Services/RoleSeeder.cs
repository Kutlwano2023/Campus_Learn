using Microsoft.AspNetCore.Identity;
using CampusLearn.Models;

namespace CampusLearn.Services
{
    public class RoleSeeder
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<Users> _userManager;

        public RoleSeeder(RoleManager<IdentityRole> roleManager, UserManager<Users> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedRolesAsync()
        {
            string[] roles = new[] { "student", "tutor", "admin" };

            foreach (var r in roles)
            {
                if (!await _roleManager.RoleExistsAsync(r))
                    await _roleManager.CreateAsync(new IdentityRole(r));
            }

            // OPTIONAL: create an admin user if none exists (helpful for first run)
            var adminEmail = "admin@campuslearn.local";
            var admin = await _userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new Users
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Campus Admin",
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}
