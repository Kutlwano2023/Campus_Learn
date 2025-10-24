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

        // Add these methods to your MessagingHub
        public async Task GetRegisteredUsers()
        {
            var currentUserId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId)) return;

            try
            {
                var users = await _mongoService.GetAllUsersAsync(currentUserId);
                var result = users.Select(u => new
                {
                    id = u.Id.ToString(),
                    fullName = u.FullName ?? "Unknown User",
                    email = u.Email ?? "No email",
                    role = u.Role ?? "User",
                    isOnline = _userConnections.ContainsKey(u.Id.ToString()),
                    hasConversation = false // We'll update this later
                }).ToList();

                await Clients.Caller.SendAsync("RegisteredUsersLoaded", result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting registered users: {ex.Message}");
                await Clients.Caller.SendAsync("UsersLoadError", "Failed to load users");
            }
        }

        // Add this method for real-time user search
        // Enhanced SearchUsers method in MessagingHub
        public async Task SearchUsers(string searchTerm)
        {
            var currentUserId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId)) return;

            try
            {
                var users = await _mongoService.SearchUsers(searchTerm, currentUserId);
                var result = users.Select(u => new
                {
                    id = u.Id.ToString(),
                    fullName = u.FullName ?? "Unknown User",
                    email = u.Email ?? "No email",
                    role = u.Role ?? "User",
                    isOnline = _userConnections.ContainsKey(u.Id.ToString()),
                    userName = u.UserName // Include username as fallback
                }).ToList();

                await Clients.Caller.SendAsync("UserSearchResults", result);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("SearchError", "Failed to search users");
                Console.WriteLine($"Error searching users: {ex.Message}");
            }
        }

        // Add method to get user by full name
        public async Task GetUserByFullName(string fullName)
        {
            var currentUserId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId)) return;

            try
            {
                var user = await _mongoService.GetUserByFullNameAsync(fullName);
                if (user != null && user.Id.ToString() != currentUserId)
                {
                    var result = new
                    {
                        id = user.Id.ToString(),
                        fullName = user.FullName ?? "Unknown User",
                        email = user.Email ?? "No email",
                        role = user.Role ?? "User",
                        isOnline = _userConnections.ContainsKey(user.Id.ToString())
                    };

                    await Clients.Caller.SendAsync("UserFoundByFullName", result);
                }
                else
                {
                    await Clients.Caller.SendAsync("UserNotFound", "User not found");
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("SearchError", "Failed to find user");
                Console.WriteLine($"Error finding user by full name: {ex.Message}");
            }
        }
        public async Task<List<string>> GetOnlineUsers()
        {
            return _userConnections.Keys.ToList();
        }

        private async Task UpdateUserOnlineStatus(string userId, bool isOnline)
        {
            await Clients.All.SendAsync("UserStatusChanged", userId, isOnline);
        }
    }
}

//sukkelnhuil