using System;
using System.Collections.Generic;

namespace CampusLearn.Models
{
    public class Conversation
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // A human readable title (optional)
        public string Title { get; set; }

        // The two participants: store their user ids (string if using ASP.NET Identity)
        public string StudentId { get; set; }
        public string TutorId { get; set; }

        // Messages navigation
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
