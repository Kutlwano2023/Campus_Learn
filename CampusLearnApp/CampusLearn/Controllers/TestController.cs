using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampusLearn.Data;
using CampusLearn.Models;

namespace CampusLearn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("chat-data")]
        public async Task<IActionResult> GetChatData()
        {
            try
            {
                var conversations = await _context.ChatConversations
                    .Include(c => c.Messages)
                    .ToListAsync();

                return Ok(new
                {
                    TotalConversations = conversations.Count,
                    Conversations = conversations.Select(c => new
                    {
                        c.ConversationId,
                        c.Title,
                        c.UserId,
                        MessageCount = c.Messages.Count,
                        LastUpdated = c.UpdatedAt,
                        Messages = c.Messages.Select(m => new
                        {
                            m.MessageId,
                            m.MessageText,
                            Sender = m.IsUserMessage ? "User" : "AI",
                            m.CreatedAt
                        })
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
                var canConnect = await _context.Database.CanConnectAsync();
                var usersCount = await _context.Users.CountAsync();
                var conversationsCount = await _context.ChatConversations.CountAsync();
                var messagesCount = await _context.ChatMessages.CountAsync();
                var materialsCount = await _context.LearningMaterials.CountAsync();

                return Ok(new
                {
                    DatabaseConnected = canConnect,
                    UsersCount = usersCount,
                    ConversationsCount = conversationsCount,
                    MessagesCount = messagesCount,
                    MaterialsCount = materialsCount,
                    Status = "Database is working correctly"
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

        [HttpGet("current-database")]
        public async Task<IActionResult> GetCurrentDatabase()
        {
            try
            {
                using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT current_database(), current_schema()";

                var result = await command.ExecuteReaderAsync();
                await result.ReadAsync();

                var dbName = result.GetString(0);
                var schema = result.GetString(1);

                return Ok(new
                {
                    Database = dbName,
                    Schema = schema,
                    ConnectionString = connection.ConnectionString
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    fullError = ex.ToString()
                });
            }
        }
    }
}