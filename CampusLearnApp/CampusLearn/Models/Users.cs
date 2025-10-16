using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampusLearn.Models
{
    public class Users : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        public string? Role { get; set; }

        private object BuildUserContext(Users user)
        {
            return new
            {
                userRole = user.Role,
                userName = user.FullName,
                userEmail = user.Email,
                platform = "CampusLearn",
                capabilities = user.Role == "Tutor" ?
                    new[] { "assignment_management", "grading", "resource_upload" } :
                    new[] { "assignment_submission", "resource_access", "progress_tracking" }
            };
        }
    }
}