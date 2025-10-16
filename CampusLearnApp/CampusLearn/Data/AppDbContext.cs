using CampusLearn.Models;
using CampusLearnApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusLearn.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Registration> Registrations { get; set; }
        public DbSet<ChatConversation> ChatConversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<LearningMaterial> LearningMaterials { get; set; }
        public DbSet<Tutor> Tutors { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure ChatConversation
            builder.Entity<ChatConversation>(entity =>
            {
                entity.HasKey(e => e.ConversationId);
                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.Title).HasMaxLength(500);
                entity.Property(e => e.ConversationType).HasMaxLength(50);

                // Relationship with Users
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ChatMessage
            builder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId);
                entity.HasIndex(e => e.ConversationId);
                entity.HasIndex(e => e.UserId);
                entity.Property(e => e.MessageType).HasMaxLength(50);

                // Relationship with ChatConversation
                entity.HasOne(e => e.Conversation)
                      .WithMany(e => e.Messages)
                      .HasForeignKey(e => e.ConversationId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relationship with Users
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure LearningMaterial
            builder.Entity<LearningMaterial>(entity =>
            {
                entity.HasKey(e => e.MaterialID);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.MaterialType).HasMaxLength(50);
                entity.Property(e => e.FilePathURL).HasMaxLength(500);
            });

            // Configure Tutor
            builder.Entity<Tutor>(entity =>
            {
                entity.HasKey(e => e.TutorID);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Specialization).HasMaxLength(200);
            });
        }
    }
}