using CampusLearn.Models;
using CampusLearn.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusLearn.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<ActionResult<ChatResponse>> SendMessage([FromBody] ChatRequest request)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var response = await _chatService.ProcessMessageAsync(request, userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending chat message");
                return StatusCode(500, "An error occurred while processing your message.");
            }
        }

        [HttpGet("conversations")]
        public async Task<ActionResult<List<ChatConversation>>> GetConversations()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                _logger.LogInformation($"Getting conversations for user: {userId}");

                var conversations = await _chatService.GetUserConversationsAsync(userId);
                _logger.LogInformation($"Found {conversations.Count} conversations for user {userId}");

                return Ok(conversations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversations for user {UserId}",
                    User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
                return StatusCode(500, $"An error occurred while retrieving conversations. Details: {ex.Message}");
            }
        }

        [HttpGet("conversations/{conversationId}")]
        public async Task<ActionResult<ChatConversation>> GetConversation(Guid conversationId)
        {
            try
            {
                var conversation = await _chatService.GetConversationWithMessagesAsync(conversationId);
                if (conversation == null)
                {
                    return NotFound();
                }

                // Verify user owns this conversation
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (conversation.UserId != userId)
                {
                    return Forbid();
                }

                return Ok(conversation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversation");
                return StatusCode(500, "An error occurred while retrieving the conversation.");
            }
        }

        [HttpDelete("conversations/{conversationId}")]
        public async Task<IActionResult> DeleteConversation(Guid conversationId)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                await _chatService.DeleteConversationAsync(conversationId, userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting conversation");
                return StatusCode(500, "An error occurred while deleting the conversation.");
            }
        }
    }
}