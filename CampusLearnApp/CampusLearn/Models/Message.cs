// Add to Models/Message.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CampusLearn.Models
{
    public class Message
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public string ConversationId { get; set; }

        // Navigation properties (not stored in MongoDB)
        [BsonIgnore]
        public Users Sender { get; set; }

        [BsonIgnore]
        public Users Receiver { get; set; }
    }

    public class Conversation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public List<string> ParticipantIds { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime LastMessageAt { get; set; }
        public string LastMessagePreview { get; set; }
        public int UnreadCount { get; set; }

        [BsonIgnore]
        public List<Users> Participants { get; set; } = new();

        [BsonIgnore]
        public List<Message> Messages { get; set; } = new();
    }
}