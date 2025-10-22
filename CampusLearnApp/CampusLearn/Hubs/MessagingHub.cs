using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CampusLearn.Hubs
{
    public class MessagingHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _userConnections = new();
        private static readonly ConcurrentDictionary<string, string> _connectionUsers = new();

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
                _connectionUsers[Context.ConnectionId] = userId;

                await Clients.All.SendAsync("UserOnline", userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_connectionUsers.TryRemove(Context.ConnectionId, out var userId))
            {
                _userConnections.TryRemove(userId, out _);
                await Clients.All.SendAsync("UserOffline", userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(senderId)) return;

            if (_userConnections.TryGetValue(receiverId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderId, message);
            }

            // Also send to sender for their own UI update
            await Clients.Caller.SendAsync("MessageSent", receiverId, message);
        }

        public async Task<string[]> SearchUsers(string searchTerm)
        {
            // This would typically query your database
            // For now, returning simulated data
            return await Task.FromResult(new[]
            {
                "user1|Alice Johnson|AJ",
                "user2|Bob Smith|BS",
                "user3|Carol Davis|CD",
                "user4|David Wilson|DW"
            }.Where(u => u.ToLower().Contains(searchTerm.ToLower())).ToArray());
        }
    }
}