using CampusLearn.Models;

namespace CampusLearn.Services
{
    public interface IChatService
    {
        Task<ChatResponse> ProcessMessageAsync(ChatRequest request, string userId);
        Task<List<ChatConversation>> GetUserConversationsAsync(string userId);
        Task<ChatConversation> GetConversationWithMessagesAsync(Guid conversationId);
        Task DeleteConversationAsync(Guid conversationId, string userId);
    }
}