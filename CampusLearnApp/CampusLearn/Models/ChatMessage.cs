using CampusLearn.Models;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CampusLearn.Models
{
    public class ChatMessage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MessageId { get; set; }

        [Required]
        public string ConversationId { get; set; }

        public string UserId { get; set; }

        [Required]
        public string MessageText { get; set; }

        [Required]
        public bool IsUserMessage { get; set; }

        [StringLength(50)]
        public string MessageType { get; set; } = "Text";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Chatbase-specific fields
        [StringLength(20)]
        public string Role { get; set; } = "user";

        public string ChatbaseMessageId { get; set; }
        public string ChatbaseChatId { get; set; }

        // JSON storage for additional data
        public string Metadata { get; set; }
    }
}