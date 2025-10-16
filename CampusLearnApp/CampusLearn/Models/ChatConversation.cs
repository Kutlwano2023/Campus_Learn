using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CampusLearn.Models
{
    public class ChatConversation
    {
        [Key]
        public Guid ConversationId { get; set; }

        [Required]
        public string UserId { get; set; }

        [StringLength(500)]
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [StringLength(50)]
        public string ConversationType { get; set; } = "General";

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual Users User { get; set; }

        public virtual ICollection<ChatMessage> Messages { get; set; }
    }

    // ChatMessage.cs
    public class ChatMessage
    {
        [Key]
        public Guid MessageId { get; set; }

        [Required]
        public Guid ConversationId { get; set; }

        public string UserId { get; set; }

        [Required]
        public string MessageText { get; set; }

        [Required]
        public bool IsUserMessage { get; set; }

        [StringLength(50)]
        public string MessageType { get; set; } = "Text";

        public DateTime CreatedAt { get; set; }

        // JSON storage for additional data
        public string Metadata { get; set; }

        // Navigation properties
        [ForeignKey("ConversationId")]
        public virtual ChatConversation Conversation { get; set; }

        [ForeignKey("UserId")]
        public virtual Users User { get; set; }
    }

    // ChatRequest.cs (DTO for API requests)
    public class ChatRequest
    {
        public string Message { get; set; }
        public Guid? ConversationId { get; set; }
        public string ConversationType { get; set; } = "General";
        public string UserRole { get; set; }
    }

    // ChatResponse.cs (DTO for API responses)
    public class ChatResponse
    {
        public Guid MessageId { get; set; }
        public string Response { get; set; }
        public Guid ConversationId { get; set; }
        public string ConversationTitle { get; set; }
        public DateTime Timestamp { get; set; }
        public List<QuickAction> SuggestedActions { get; set; }
    }

    // QuickAction.cs
    public class QuickAction
    {
        public string Text { get; set; }
        public string Action { get; set; }
        public string Type { get; set; } // "button", "link", "query"
    }
}