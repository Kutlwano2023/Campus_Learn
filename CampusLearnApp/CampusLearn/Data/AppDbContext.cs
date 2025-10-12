using CampusLearn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic;

namespace CampusLearn.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Conversation>(b =>
            {
                b.HasKey(c => c.Id);
                b.HasMany(c => c.Messages).WithOne(m => m.Conversation).HasForeignKey(m => m.ConversationId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasKey(m => m.Id);
                b.Property(m => m.Text).IsRequired();
            });
        }
    }
}
