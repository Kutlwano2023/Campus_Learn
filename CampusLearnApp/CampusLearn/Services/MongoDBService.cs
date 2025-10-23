using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using CampusLearn.Models;

namespace CampusLearn.Services
{
    public class MongoService
    {
        private readonly IMongoCollection<Users> _users;
        private readonly IMongoCollection<Message> _messages;
        private readonly IMongoCollection<Conversation> _conversations;
        private readonly IMongoCollection<Resource> _resources;

        public MongoService(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);

            _users = database.GetCollection<Users>("users");
            _messages = database.GetCollection<Message>("messages");
            _conversations = database.GetCollection<Conversation>("conversations");
            _resources = database.GetCollection<Resource>("resources"); // Added this line
        }

        // User methods
        // Enhanced SearchUsers method in MongoService
        public async Task<List<Users>> SearchUsers(string searchTerm, string currentUserId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    // If no search term, return all users except current user
                    return await GetAllUsersAsync(currentUserId);
                }

                // Prioritize full name search, then email, then username
                var filter = Builders<Users>.Filter.And(
                    Builders<Users>.Filter.Ne(u => u.Id.ToString(), currentUserId),
                    Builders<Users>.Filter.Or(
                        Builders<Users>.Filter.Regex(u => u.FullName, new BsonRegularExpression(searchTerm, "i")),
                        Builders<Users>.Filter.Regex(u => u.Email, new BsonRegularExpression(searchTerm, "i")),
                        Builders<Users>.Filter.Regex(u => u.UserName, new BsonRegularExpression(searchTerm, "i"))
                    )
                );

                var users = await _users.Find(filter)
                    .SortBy(u => u.FullName) // Sort by full name
                    .Limit(20)
                    .ToListAsync();

                // Prioritize results that match full name at the beginning
                return users.OrderByDescending(u =>
                    u.FullName?.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase) == true)
                    .ThenBy(u => u.FullName)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchUsers: {ex.Message}");
                return new List<Users>();
            }
        }

        // Add method to get user by full name
        public async Task<Users> GetUserByFullNameAsync(string fullName)
        {
            try
            {
                return await _users.Find(u => u.FullName == fullName).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user by full name: {ex.Message}");
                return null;
            }
        }

        // Enhanced method to get conversations with full user details
        public async Task<List<Conversation>> GetUserConversationsAsync(string userId)
        {
            try
            {
                var filter = Builders<Conversation>.Filter.AnyEq(c => c.ParticipantIds, userId);
                var conversations = await _conversations.Find(filter)
                    .SortByDescending(c => c.LastMessageAt)
                    .ToListAsync();

                // Populate participant details with full names
                foreach (var conversation in conversations)
                {
                    var otherParticipantId = conversation.ParticipantIds.FirstOrDefault(id => id != userId);
                    if (!string.IsNullOrEmpty(otherParticipantId))
                    {
                        var otherUser = await GetUserByIdAsync(otherParticipantId);
                        if (otherUser != null)
                        {
                            conversation.Participants = new List<Users> { otherUser };
                        }
                        else
                        {
                            // Create a placeholder with the ID as reference
                            conversation.Participants = new List<Users> {
                        new Users {
                            Id = Guid.Parse(otherParticipantId),
                            FullName = "Unknown User",
                            Email = "unknown@example.com"
                        }
                    };
                        }
                    }
                }

                return conversations;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetUserConversationsAsync: {ex.Message}");
                return new List<Conversation>();
            }
        }
        public async Task<Users> GetUserByIdAsync(string userId)
        {
            return await _users.Find(u => u.Id.ToString() == userId).FirstOrDefaultAsync();
        }

        public async Task<Users> GetUserByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        // Conversation methods
        public async Task<Conversation> GetOrCreateConversationAsync(string user1Id, string user2Id)
        {
            var participants = new List<string> { user1Id, user2Id }.OrderBy(id => id).ToList();
            var filter = Builders<Conversation>.Filter.All(c => c.ParticipantIds, participants);

            var conversation = await _conversations.Find(filter).FirstOrDefaultAsync();

            if (conversation == null)
            {
                conversation = new Conversation
                {
                    ParticipantIds = participants,
                    CreatedAt = DateTime.UtcNow,
                    LastMessageAt = DateTime.UtcNow,
                    LastMessagePreview = "Conversation started",
                    UnreadCount = 0
                };

                await _conversations.InsertOneAsync(conversation);
            }

            return conversation;
        }

        // Add these methods to your MongoService class
        public async Task<List<Users>> GetAllUsersAsync(string excludeUserId = null)
        {
            try
            {
                var filter = string.IsNullOrEmpty(excludeUserId)
                    ? Builders<Users>.Filter.Empty
                    : Builders<Users>.Filter.Ne(u => u.Id.ToString(), excludeUserId);

                return await _users.Find(filter)
                    .SortBy(u => u.FullName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all users: {ex.Message}");
                return new List<Users>();
            }
        }

        public async Task<List<Message>> GetConversationMessagesAsync(string conversationId, int limit = 50)
        {
            return await _messages.Find(m => m.ConversationId == conversationId)
                .SortByDescending(m => m.SentAt)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<List<Message>> GetMessagesBetweenUsers(string user1Id, string user2Id)
        {
            var conversation = await GetOrCreateConversationAsync(user1Id, user2Id);
            return await GetConversationMessagesAsync(conversation.Id);
        }

        public async Task<Message> SaveMessageAsync(Message message)
        {
            // Ensure conversation exists
            var conversation = await GetOrCreateConversationAsync(message.SenderId, message.ReceiverId);
            message.ConversationId = conversation.Id;
            message.SentAt = DateTime.UtcNow;

            await _messages.InsertOneAsync(message);

            // Update conversation last message
            var update = Builders<Conversation>.Update
                .Set(c => c.LastMessageAt, message.SentAt)
                .Set(c => c.LastMessagePreview, message.Content.Length > 50
                    ? message.Content.Substring(0, 50) + "..."
                    : message.Content)
                .Inc(c => c.UnreadCount, 1);

            await _conversations.UpdateOneAsync(c => c.Id == conversation.Id, update);

            return message;
        }

        public async Task MarkMessagesAsReadAsync(string conversationId, string userId)
        {
            var filter = Builders<Message>.Filter.And(
                Builders<Message>.Filter.Eq(m => m.ConversationId, conversationId),
                Builders<Message>.Filter.Eq(m => m.ReceiverId, userId),
                Builders<Message>.Filter.Eq(m => m.IsRead, false)
            );

            var update = Builders<Message>.Update.Set(m => m.IsRead, true);
            await _messages.UpdateManyAsync(filter, update);

            // Reset unread count for this conversation
            var conversationUpdate = Builders<Conversation>.Update.Set(c => c.UnreadCount, 0);
            await _conversations.UpdateOneAsync(c => c.Id == conversationId, conversationUpdate);
        }

        public async Task<int> GetUnreadMessageCountAsync(string userId)
        {
            var filter = Builders<Message>.Filter.And(
                Builders<Message>.Filter.Eq(m => m.ReceiverId, userId),
                Builders<Message>.Filter.Eq(m => m.IsRead, false)
            );

            return (int)await _messages.CountDocumentsAsync(filter);
        }

        // Resource methods
        public async Task<List<Resource>> GetActiveResourcesAsync()
        {
            return await _resources.Find(r => r.IsActive)
                .SortByDescending(r => r.UploadDate)
                .ToListAsync();
        }

        public async Task<List<Resource>> SearchResourcesAsync(string query, string type, string fileType)
        {
            var filter = Builders<Resource>.Filter.Where(r => r.IsActive);

            if (!string.IsNullOrEmpty(query))
            {
                filter &= Builders<Resource>.Filter.Or(
                    Builders<Resource>.Filter.Regex(r => r.Title, new BsonRegularExpression(query, "i")),
                    Builders<Resource>.Filter.Regex(r => r.Description, new BsonRegularExpression(query, "i")),
                    Builders<Resource>.Filter.Regex(r => r.Author, new BsonRegularExpression(query, "i"))
                );
            }

            if (!string.IsNullOrEmpty(type))
            {
                filter &= Builders<Resource>.Filter.Eq(r => r.Type, type);
            }

            if (!string.IsNullOrEmpty(fileType))
            {
                filter &= Builders<Resource>.Filter.Eq(r => r.FileType, fileType);
            }

            return await _resources.Find(filter)
                .SortByDescending(r => r.UploadDate)
                .ToListAsync();
        }

        public async Task<Resource> GetResourceByIdAsync(string id)
        {
            return await _resources.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateResourceDownloadsAsync(string id)
        {
            var update = Builders<Resource>.Update.Inc(r => r.Downloads, 1);
            await _resources.UpdateOneAsync(r => r.Id == id, update);
        }

        public async Task SoftDeleteResourceAsync(string id)
        {
            var update = Builders<Resource>.Update.Set(r => r.IsActive, false);
            await _resources.UpdateOneAsync(r => r.Id == id, update);
        }

        // Property accessors for other parts of your application
        public IMongoCollection<Users> UsersCollection => _users;
        public IMongoCollection<Message> MessagesCollection => _messages;
        public IMongoCollection<Conversation> ConversationsCollection => _conversations;
        public IMongoCollection<Resource> Resources => _resources;

        // Backward compatibility properties
        public IMongoCollection<Users> Users => _users; // If you need this
        public IMongoCollection<Conversation> ChatConversations => _conversations;
        public IMongoCollection<Message> ChatMessages => _messages;
    }
}