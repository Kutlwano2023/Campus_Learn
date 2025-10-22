using CampusLearn.Models;
using CampusLearn.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusLearn.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ConversationController> _logger;

        public ConversationController(IChatService chatService, ILogger<ConversationController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ChatConversation>>> GetConversations()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var conversations = await _chatService.GetUserConversationsAsync(userId);
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving conversations");
                return StatusCode(500, "An error occurred while retrieving conversations.");
            }
        }

        [HttpGet("{conversationId}")]
        public async Task<ActionResult<ChatConversation>> GetConversation(string conversationId)
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
    }
}