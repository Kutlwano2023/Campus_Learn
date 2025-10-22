using CampusLearn.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace CampusLearn.Services
{
    public class MongoService
    {
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<Message> _messagesCollection;
        private readonly UserManager<Users> _userManager;

        public MongoService(IOptions<MongoDBSettings> settings, UserManager<Users> userManager)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _db = client.GetDatabase(settings.Value.DatabaseName);
            _messagesCollection = _db.GetCollection<Message>("messages");
            _userManager = userManager;
        }

        public async Task<List<Conversation>> GetUserConversations(string userId)
        {
            var messages = await _messagesCollection
                .Find(m => m.SenderId == userId || m.ReceiverId == userId)
                .SortByDescending(m => m.SentAt)
                .ToListAsync();

            var conversationDict = new Dictionary<string, Conversation>();

            foreach (var message in messages)
            {
                var otherUserId = message.SenderId == userId ? message.ReceiverId : message.SenderId;

                if (!conversationDict.ContainsKey(otherUserId))
                {
                    var otherUser = await _userManager.FindByIdAsync(otherUserId);
                    conversationDict[otherUserId] = new Conversation
                    {
                        OtherUserId = otherUserId,
                        OtherUserName = otherUser?.FullName ?? "Unknown User",
                        LastMessage = message.Content,
                        LastMessageTime = message.SentAt,
                        UnreadCount = await GetUnreadCountForUser(userId, otherUserId)
                    };
                }
            }

            return conversationDict.Values
                .OrderByDescending(c => c.LastMessageTime)
                .ToList();
        }

        private async Task<int> GetUnreadCountForUser(string userId, string otherUserId)
        {
            return (int)await _messagesCollection
                .CountDocumentsAsync(m => m.ReceiverId == userId &&
                                        m.SenderId == otherUserId &&
                                        !m.IsRead);
        }

        public async Task<List<Users>> SearchUsers(string query, string currentUserId)
        {
            // Convert string currentUserId to Guid for comparison
            var currentUserGuid = Guid.Parse(currentUserId);

            var users = _userManager.Users
                .AsQueryable()
                .Where(u => u.Id != currentUserGuid &&
                           (u.FullName.Contains(query) || u.Email.Contains(query)))
                .Take(10)
                .ToList();

            return await Task.FromResult(users);
        }

        public async Task<List<Message>> GetMessagesBetweenUsers(string user1Id, string user2Id)
        {
            return await _messagesCollection
                .Find(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                           (m.SenderId == user2Id && m.ReceiverId == user1Id))
                .SortBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task SaveMessage(Message message)
        {
            await _messagesCollection.InsertOneAsync(message);
        }

        public async Task MarkMessagesAsRead(string userId, string otherUserId)
        {
            var filter = Builders<Message>.Filter.And(
                Builders<Message>.Filter.Eq(m => m.ReceiverId, userId),
                Builders<Message>.Filter.Eq(m => m.SenderId, otherUserId),
                Builders<Message>.Filter.Eq(m => m.IsRead, false)
            );

            var update = Builders<Message>.Update.Set(m => m.IsRead, true);
            await _messagesCollection.UpdateManyAsync(filter, update);
        }

        public async Task<int> GetUnreadMessageCount(string userId)
        {
            return (int)await _messagesCollection
                .CountDocumentsAsync(m => m.ReceiverId == userId && !m.IsRead);
        }

        // Identity and User collections
        public IMongoCollection<Users> Users => _db.GetCollection<Users>("Users");

        // Chat collections
        public IMongoCollection<ChatConversation> ChatConversations => _db.GetCollection<ChatConversation>("ChatConversations");
        public IMongoCollection<ChatMessage> ChatMessages => _db.GetCollection<ChatMessage>("ChatMessages");

        // Academic collections
        public IMongoCollection<Resource> Resources => _db.GetCollection<Resource>("Resources");
        public IMongoCollection<Tutor> Tutors => _db.GetCollection<Tutor>("Tutors");
        public IMongoCollection<Topic> Topics => _db.GetCollection<Topic>("Topics");
        public IMongoCollection<Submission> Submissions => _db.GetCollection<Submission>("Submissions");
        public IMongoCollection<Module> Modules => _db.GetCollection<Module>("Modules");
        public IMongoCollection<Enrollment> Enrollments => _db.GetCollection<Enrollment>("Enrollments");
        public IMongoCollection<Review> Reviews => _db.GetCollection<Review>("Reviews");
        public IMongoCollection<Student> Students => _db.GetCollection<Student>("Students");

        // Assessment and Report collections
        public IMongoCollection<Assessment> Assessments => _db.GetCollection<Assessment>("Assessments");
        public IMongoCollection<Report> Reports => _db.GetCollection<Report>("Reports");

        // Learning Material collection
        public IMongoCollection<LearningMaterial> LearningMaterials => _db.GetCollection<LearningMaterial>("LearningMaterials");
    }
}