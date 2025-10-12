using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CampusLearn.Services;
using Microsoft.AspNetCore.Authorization;

namespace CampusLearn.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // GET api/chat/conversation/{studentId}/{tutorId}
        [HttpGet("conversation/{studentId}/{tutorId}")]
        public async Task<IActionResult> GetOrCreate(string studentId, string tutorId)
        {
            var conv = await _chatService.GetOrCreateConversationAsync(studentId, tutorId);
            return Ok(conv);
        }

        [HttpGet("conversation/{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(Guid conversationId, int page = 0, int pageSize = 50)
        {
            var messages = await _chatService.GetMessagesAsync(conversationId, page, pageSize);
            return Ok(messages);
        }

        // Optional: post a message via REST (alternatively use SignalR)
        [HttpPost("{conversationId}/message")]
        public async Task<IActionResult> PostMessage(Guid conversationId, [FromBody] PostMessageRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.SenderId) || string.IsNullOrWhiteSpace(req.Text))
                return BadRequest("Missing fields.");

            var msg = await _chatService.AddMessageAsync(conversationId, req.SenderId, req.SenderName, req.Text);
            return Ok(msg);
        }
    }

    public class PostMessageRequest
    {
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string Text { get; set; }
    }
}
