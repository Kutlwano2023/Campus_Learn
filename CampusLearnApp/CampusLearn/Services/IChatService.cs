using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLearn.Models;

namespace CampusLearn.Services
{
    public interface IChatService
    {
        Task<Conversation> GetOrCreateConversationAsync(string studentId, string tutorId);
        Task<Conversation> GetConversationAsync(Guid conversationId);
        Task<IEnumerable<ChatMessage>> GetMessagesAsync(Guid conversationId, int page = 0, int pageSize = 50);
        Task<ChatMessage> AddMessageAsync(Guid conversationId, string senderId, string senderName, string text);
    }
}
