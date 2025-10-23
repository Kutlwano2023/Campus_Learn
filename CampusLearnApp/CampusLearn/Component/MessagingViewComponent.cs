using Microsoft.AspNetCore.Mvc;
using CampusLearn.Services;
using Microsoft.AspNetCore.Identity;
using CampusLearn.Models;
using System.Security.Claims;

namespace CampusLearn.ViewComponents
{
    public class MessagingViewComponent : ViewComponent
    {
        private readonly MongoService _mongoService;
        private readonly UserManager<Users> _userManager;

        public MessagingViewComponent(MongoService mongoService, UserManager<Users> userManager)
        {
            _mongoService = mongoService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = UserClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return View(new List<Conversation>());
            }

            try
            {
                var conversations = await _mongoService.GetUserConversationsAsync(userId);
                return View(conversations);
            }
            catch (Exception ex)
            {
                // Log error and return empty list
                Console.WriteLine($"Error loading conversations: {ex.Message}");
                return View(new List<Conversation>());
            }
        }
    }
}