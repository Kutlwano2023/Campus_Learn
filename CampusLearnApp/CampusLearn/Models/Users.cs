using Microsoft.AspNetCore.Identity;
namespace CampusLearn.Models
{
    public class Users: IdentityUser
    {

        
        public string Fullname{ get; set; }

        public string  Role { get; set; }=" Student";
      

    }
}
