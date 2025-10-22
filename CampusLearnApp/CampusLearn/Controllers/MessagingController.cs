using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CampusLearn.Models;
using CampusLearn.Services;
using System.Security.Claims;

namespace CampusLearn.Controllers
{
    [ApiController]
    [Route("api/messaging")]
    [Authorize]
    public class MessagingController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly MongoService _mongoService;

        public MessagingController(UserManager<Users> userManager, MongoService mongoService)
        {
            _userManager = userManager;
            _mongoService = mongoService;
        }

        [HttpGet("auth-status")]
        public IActionResult GetAuthStatus()
        {
            return Ok(new { isAuthenticated = User.Identity.IsAuthenticated });
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            var conversations = await _mongoService.GetUserConversations(currentUser.Id.ToString());
            return Ok(conversations);
        }

        [HttpGet("search-users")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return Ok(new List<object>());

            var currentUser = await _userManager.GetUserAsync(User);
            var users = await _mongoService.SearchUsers(query, currentUser.Id.ToString());

            return Ok(users.Select(u => new
            {
                id = u.Id.ToString(),
                fullName = u.FullName,
                email = u.Email,
                role = u.Role
            }));
        }

        [HttpGet("user-info/{userId}")]
        public async Task<IActionResult> GetUserInfo(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            return Ok(new
            {
                id = user.Id.ToString(),
                fullName = user.FullName,
                email = user.Email,
                role = user.Role
            });
        }

        [HttpGet("messages/{userId}")]
        public async Task<IActionResult> GetMessages(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var messages = await _mongoService.GetMessagesBetweenUsers(currentUser.Id.ToString(), userId);

            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var message = new Message
            {
                SenderId = currentUser.Id.ToString(),
                ReceiverId = request.ReceiverId,
                Content = request.Content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _mongoService.SaveMessage(message);
            return Ok();
        }

        [HttpPost("mark-read/{userId}")]
        public async Task<IActionResult> MarkAsRead(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            await _mongoService.MarkMessagesAsRead(currentUser.Id.ToString(), userId);
            return Ok();
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var count = await _mongoService.GetUnreadMessageCount(currentUser.Id.ToString());
            return Ok(new { count });
        }
    }

    public class SendMessageRequest
    {
        public string ReceiverId { get; set; }
        public string Content { get; set; }
    }
}