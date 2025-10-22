using System;

namespace CampusLearn.Models
{
    public class Login
    {
        public string LoginId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }

        public void UpdatePassword()
        {
            //logic
        }

        public void ReceiveReport()
        {
            //logic
        }
    }
}