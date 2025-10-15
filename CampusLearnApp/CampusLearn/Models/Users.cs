using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Models
{
    public class Users : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]  
        [StringLength(50)]
        public string Role { get; set; }
    }
}
