using System.ComponentModel.DataAnnotations;

namespace CampusLearn.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Display(Name = "Verification Code")]
        [StringLength(6, ErrorMessage = "Verification code must be 6 characters.", MinimumLength = 6)]
        public string VerificationCode { get; set; }

        public bool IsCodeSent { get; set; }
        public string Message { get; set; }
    }
}