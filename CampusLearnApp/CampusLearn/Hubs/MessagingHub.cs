using Microsoft.AspNetCore.SignalR;
using CampusLearn.Models;
using CampusLearn.Services;
using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;

namespace CampusLearn.Hubs
{
    public class MessagingHub : Hub
    {
        private readonly MongoService _mongoService;
        private readonly UserManager<Users> _userManager;
        private static readonly ConcurrentDictionary<string, string> _userConnections = new();
        private static readonly ConcurrentDictionary<string, string> _connectionUsers = new();

        public MessagingHub(MongoService mongoService, UserManager<Users> userManager)
        {
            _mongoService = mongoService;
            _userManager = userManager;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
                _connectionUsers[Context.ConnectionId] = userId;
                await UpdateUserOnlineStatus(userId, true);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connectionUsers.TryRemove(Context.ConnectionId, out var userId))
            {
                _userConnections.TryRemove(userId, out _);
                await UpdateUserOnlineStatus(userId, false);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string receiverId, string content)
        {
            var senderId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId)) return;

            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            var savedMessage = await _mongoService.SaveMessageAsync(message);

            // Notify receiver if online
            if (_userConnections.TryGetValue(receiverId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", savedMessage);
            }

            // Notify sender for their own UI update
            await Clients.Caller.SendAsync("MessageSent", savedMessage);
        }

        public async Task MarkAsRead(string conversationId)
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await _mongoService.MarkMessagesAsReadAsync(conversationId, userId);
                await Clients.Caller.SendAsync("MessagesRead", conversationId);
            }
        }

        public async Task<List<Users>> SearchUsers(string searchTerm)
        {
            var currentUserId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId)) return new List<Users>();

            var users = await _mongoService.SearchUsers(searchTerm, currentUserId);
            return users;
        }

        private async Task UpdateUserOnlineStatus(string userId, bool isOnline)
        {
            await Clients.All.SendAsync("UserStatusChanged", userId, isOnline);
        }
    }
}