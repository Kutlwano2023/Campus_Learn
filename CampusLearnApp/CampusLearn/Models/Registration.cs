using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Models;
public class Registration
{
    [Required]
    public string FullName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }

    [Required]
    public string Role { get; set; }

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
