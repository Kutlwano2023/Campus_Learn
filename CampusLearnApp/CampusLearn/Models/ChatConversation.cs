using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CampusLearn.Models
{
    public class ChatConversation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ConversationId { get; set; }

        [Required]
        public string UserId { get; set; }

        [StringLength(500)]
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string ConversationType { get; set; } = "General";

        // Chatbase-specific fields
        public string ChatbaseChatId { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active";
    }

    public class ChatResponse
    {
        public string MessageId { get; set; }
        public string Response { get; set; }
        public string ConversationId { get; set; }
        public string ConversationTitle { get; set; }
        public DateTime Timestamp { get; set; }
        public List<QuickAction> SuggestedActions { get; set; }
    }

    public class QuickAction
    {
        public string Text { get; set; }
        public string Action { get; set; }
        public string Type { get; set; } // "button", "link", "query"
    }

    public class ChatRequest
    {
        public string Message { get; set; }
        public string ConversationId { get; set; }
        public string ConversationType { get; set; } = "General";
        public string UserRole { get; set; }
    }

    // Chatbase API Models
    public class ChatbaseApiRequest
    {
        public List<ChatbaseMessage> messages { get; set; }
        public string chatId { get; set; }
        public bool stream { get; set; } = false;
        public string userId { get; set; }
        public object userData { get; set; }
    }

    public class ChatbaseMessage
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class ChatbaseApiResponse
    {
        public string text { get; set; }
        public List<object> sources { get; set; }
        public string chatId { get; set; }
        public string messageId { get; set; }
        public string sessionId { get; set; }
    }

    public class ChatbaseWebhookPayload
    {
        public string ChatId { get; set; }
        public string UserId { get; set; }
        public ChatbaseWebhookMessage Message { get; set; }
        public ChatbaseWebhookMessage Response { get; set; }
        public long Timestamp { get; set; }
    }

    public class ChatbaseWebhookMessage
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Role { get; set; }
    }
}