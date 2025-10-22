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
                var conversations = await _mongoService.ChatConversations.Find(_ => true).ToListAsync();
                var messages = await _mongoService.ChatMessages.Find(_ => true).ToListAsync();
                var users = await _mongoService.Users.Find(_ => true).ToListAsync();

                return Ok(new
                {
                    TotalConversations = conversations.Count,
                    TotalMessages = messages.Count,
                    TotalUsers = users.Count,
                    Conversations = conversations.Select(c => new
                    {
                        c.ConversationId,
                        c.Title,
                        c.UserId,
                        c.ConversationType,
                        c.ChatbaseChatId,
                        c.Status,
                        MessageCount = messages.Count(m => m.ConversationId == c.ConversationId),
                        LastUpdated = c.UpdatedAt,
                        Created = c.CreatedAt
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
                var usersCount = await _mongoService.Users.CountDocumentsAsync(_ => true);
                var conversationsCount = await _mongoService.ChatConversations.CountDocumentsAsync(_ => true);
                var messagesCount = await _mongoService.ChatMessages.CountDocumentsAsync(_ => true);

                return Ok(new
                {
                    DatabaseConnected = true,
                    UsersCount = usersCount,
                    ConversationsCount = conversationsCount,
                    MessagesCount = messagesCount,
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
                var databaseStatus = await _mongoService.Users.Find(_ => true).AnyAsync();
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
    }
}