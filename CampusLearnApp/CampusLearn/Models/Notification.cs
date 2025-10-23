using System;

namespace CampusLearn.Models
{
    public enum NotificationType
    {
        Announcement,
        Connection,
        Message,
        UpcomingQuiz,
        IncompleteQuiz
    }

    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty; // the user who receives the notification
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Link { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}