using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CampusLearn.Services;
using Microsoft.AspNetCore.Authorization;

namespace CampusLearn.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCount()
        {
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User?.Identity?.Name ?? "anonymous";
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Json(new { unread = count });
        }

        [HttpGet]
        public async Task<IActionResult> GetRecent(int limit = 20)
        {
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User?.Identity?.Name ?? "anonymous";
            var items = await _notificationService.GetRecentAsync(userId, limit);
            return Json(items);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead([FromBody] Guid id)
        {
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User?.Identity?.Name ?? "anonymous";
            await _notificationService.MarkAsReadAsync(userId, id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                         ?? User?.Identity?.Name ?? "anonymous";
            var items = await _notificationService.GetAllAsync(userId);
            return View(items);
        }
    }
}