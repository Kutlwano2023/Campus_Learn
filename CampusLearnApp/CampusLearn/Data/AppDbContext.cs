using CampusLearn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CampusLearn.Data
{
    public class AppDbContext: IdentityDbContext<Users,IdentityRole,string>
    {
        public AppDbContext(DbContextOptions options) : base(options) { 
           
        }
    }
}
