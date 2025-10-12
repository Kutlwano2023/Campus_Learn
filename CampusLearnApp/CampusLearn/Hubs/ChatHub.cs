using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using CampusLearn.Services;
using CampusLearn.Models;

namespace CampusLearn.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        // Called when a client connects to a conversation room
        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        // Send message to conversation: store it, then broadcast to group
        public async Task SendMessage(string conversationId, string senderId, string senderName, string text)
        {
            // Save message through service
            var message = await _chatService.AddMessageAsync(Guid.Parse(conversationId), senderId, senderName, text);

            // Broadcast message to everyone in the conversation
            await Clients.Group(conversationId).SendAsync("ReceiveMessage", new
            {
                id = message.Id,
                conversationId = message.ConversationId,
                senderId = message.SenderId,
                senderName = message.SenderName,
                text = message.Text,
                sentAt = message.SentAt
            });
        }
    }
}
