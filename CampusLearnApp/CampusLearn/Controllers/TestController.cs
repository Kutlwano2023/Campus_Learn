using Microsoft.AspNetCore.Mvc;
using CampusLearn.Models;
using CampusLearn.Services;
using System.Text.Json;
using MongoDB.Driver;

namespace CampusLearn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly MongoService _mongoService;
        private readonly IChatService _chatService;
        private readonly IConfiguration _configuration;

        public TestController(MongoService mongoService, IChatService chatService, IConfiguration configuration)
        {
            _mongoService = mongoService;
            _chatService = chatService;
            _configuration = configuration;
        }

        [HttpGet("chat-data")]
        public async Task<IActionResult> GetChatData()
        {
            try
            {
                // Use the correct collection names from MongoService
                var conversations = await _mongoService.ConversationsCollection.Find(_ => true).ToListAsync();
                var messages = await _mongoService.MessagesCollection.Find(_ => true).ToListAsync();
                var users = await _mongoService.UsersCollection.Find(_ => true).ToListAsync();

                return Ok(new
                {
                    TotalConversations = conversations.Count,
                    TotalMessages = messages.Count,
                    TotalUsers = users.Count,
                    Conversations = conversations.Select(c => new
                    {
                        c.Id,
                        ParticipantIds = c.ParticipantIds,
                        LastMessageAt = c.LastMessageAt,
                        LastMessagePreview = c.LastMessagePreview,
                        UnreadCount = c.UnreadCount,
                        CreatedAt = c.CreatedAt,
                        MessageCount = messages.Count(m => m.ConversationId == c.Id),
                        Participants = c.Participants?.Select(p => new { p.Id, p.FullName, p.Email })
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, details = ex.StackTrace });
            }
        }

        [HttpGet("database-status")]
        public async Task<IActionResult> GetDatabaseStatus()
        {
            try
            {
                var usersCount = await _mongoService.UsersCollection.CountDocumentsAsync(_ => true);
                var conversationsCount = await _mongoService.ConversationsCollection.CountDocumentsAsync(_ => true);
                var messagesCount = await _mongoService.MessagesCollection.CountDocumentsAsync(_ => true);
                var resourcesCount = await _mongoService.Resources.CountDocumentsAsync(_ => true);

                return Ok(new
                {
                    DatabaseConnected = true,
                    UsersCount = usersCount,
                    ConversationsCount = conversationsCount,
                    MessagesCount = messagesCount,
                    ResourcesCount = resourcesCount,
                    Status = "MongoDB is working correctly"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message,
                    Details = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("health")]
        public async Task<IActionResult> HealthCheck()
        {
            try
            {
                var databaseStatus = await _mongoService.UsersCollection.Find(_ => true).AnyAsync();
                var chatbaseConfig = _configuration.GetSection("ChatBase");
                var hasChatbaseConfig = !string.IsNullOrEmpty(chatbaseConfig["ApiKey"]);

                var healthStatus = new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Database = databaseStatus ? "Connected" : "Disconnected",
                    Chatbase = hasChatbaseConfig ? "Configured" : "Not Configured",
                    Services = new
                    {
                        Database = true,
                        ChatService = _chatService != null,
                        Configuration = true
                    },
                    Collections = new
                    {
                        Users = true,
                        Conversations = true,
                        Messages = true,
                        Resources = true
                    }
                };

                return Ok(healthStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(503, new
                {
                    Status = "Unhealthy",
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
            }
        }

        [HttpGet("user-conversations/{userId}")]
        public async Task<IActionResult> GetUserConversations(string userId)
        {
            try
            {
                var conversations = await _mongoService.GetUserConversationsAsync(userId);
                var messages = await _mongoService.MessagesCollection.Find(_ => true).ToListAsync();

                return Ok(new
                {
                    UserId = userId,
                    Conversations = conversations.Select(c => new
                    {
                        ConversationId = c.Id,
                        Participants = c.Participants?.Select(p => new { p.Id, p.FullName, p.Email }),
                        LastMessage = c.LastMessagePreview,
                        LastMessageAt = c.LastMessageAt,
                        UnreadCount = c.UnreadCount,
                        TotalMessages = messages.Count(m => m.ConversationId == c.Id)
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("recent-messages")]
        public async Task<IActionResult> GetRecentMessages(int limit = 20)
        {
            try
            {
                var messages = await _mongoService.MessagesCollection
                    .Find(_ => true)
                    .SortByDescending(m => m.SentAt)
                    .Limit(limit)
                    .ToListAsync();

                // Get user details for each message
                var userIds = messages.SelectMany(m => new[] { m.SenderId, m.ReceiverId }).Distinct().ToList();
                var users = await _mongoService.UsersCollection
                    .Find(u => userIds.Contains(u.Id.ToString()))
                    .ToListAsync();

                var userDict = users.ToDictionary(u => u.Id.ToString(), u => u);

                return Ok(new
                {
                    TotalMessages = messages.Count,
                    Messages = messages.Select(m => new
                    {
                        m.Id,
                        m.Content,
                        m.SentAt,
                        m.IsRead,
                        Sender = userDict.ContainsKey(m.SenderId) ? new { userDict[m.SenderId].FullName, userDict[m.SenderId].Email } : null,
                        Receiver = userDict.ContainsKey(m.ReceiverId) ? new { userDict[m.ReceiverId].FullName, userDict[m.ReceiverId].Email } : null,
                        m.ConversationId
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}