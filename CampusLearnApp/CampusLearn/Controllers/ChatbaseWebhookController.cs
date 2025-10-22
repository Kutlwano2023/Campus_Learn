using CampusLearn.Models;
using CampusLearn.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

namespace CampusLearn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbaseWebhookController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatbaseWebhookController> _logger;

        public ChatbaseWebhookController(IChatService chatService, ILogger<ChatbaseWebhookController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpPost("message")]
        public async Task<IActionResult> HandleMessage([FromBody] ChatbaseWebhookPayload payload)
        {
            try
            {
                _logger.LogInformation($"Received Chatbase webhook: {JsonSerializer.Serialize(payload)}");

                if (payload == null)
                {
                    return BadRequest("Invalid payload");
                }

                _logger.LogInformation($"Webhook received for user {payload.UserId}, chat ID: {payload.ChatId}");

                return Ok(new { success = true, message = "Webhook received" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling Chatbase webhook");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }

    public class ChatbaseWebhookPayload
    {
        public string ChatId { get; set; }
        public string UserId { get; set; }
        public ChatbaseMessage Message { get; set; }
        public ChatbaseMessage Response { get; set; }
        public long Timestamp { get; set; }
    }

    public class ChatbaseMessage
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Role { get; set; }
    }
}