using Microsoft.AspNetCore.Identity;
using CampusLearn.Models;

namespace CampusLearn.Services
{
    public class RoleSeeder
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleSeeder(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedRolesAsync()
        {
            string[] roles = { "ADMIN", "TUTOR", "STUDENT" }; // Your seeded roles

            // Also create the role variations used in registration
            string[] additionalRoles = { "Student", "Tutor", "Instructor", "Admin" };

            var allRoles = roles.Union(additionalRoles.Select(r => r.ToUpper()));

            foreach (var role in allRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new ApplicationRole(role));
                }
            }
        }
    }
}
