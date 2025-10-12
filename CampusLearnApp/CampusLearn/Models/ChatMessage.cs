using System;

namespace CampusLearn.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // FK
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        // Sender (store user id string - adjust if you use int ids)
        public string SenderId { get; set; }

        // Sender display name (cached)
        public string SenderName { get; set; }

        public string Text { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // whether message was read by recipient
        public bool IsRead { get; set; } = false;
    }
}
