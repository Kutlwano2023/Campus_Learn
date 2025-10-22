using CampusLearn.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CampusLearn.Services
{
    public interface IChatService
    {
        Task<ChatResponse> ProcessMessageAsync(ChatRequest request, string userId);
        Task<List<ChatConversation>> GetUserConversationsAsync(string userId);
        Task<ChatConversation> GetConversationWithMessagesAsync(string conversationId);
        Task DeleteConversationAsync(string conversationId, string userId);
    }
}