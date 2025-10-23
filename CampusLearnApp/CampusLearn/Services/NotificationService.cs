using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLearn.Models;

namespace CampusLearn.Services
{
    // Simple in-memory demo service. Replace with DB-backed calls later.
    public class NotificationService : INotificationService
    {
        private static readonly ConcurrentDictionary<string, ConcurrentBag<Notification>> _store =
            new ConcurrentDictionary<string, ConcurrentBag<Notification>>();

        public Task<int> GetUnreadCountAsync(string userId)
        {
            var bag = GetBag(userId);
            return Task.FromResult(bag.Count(n => !n.IsRead));
        }

        public Task<IEnumerable<Notification>> GetRecentAsync(string userId, int limit = 20)
        {
            var bag = GetBag(userId);
            var items = bag.OrderByDescending(n => n.CreatedAtUtc).Take(limit).AsEnumerable();
            return Task.FromResult(items);
        }

        public Task<IEnumerable<Notification>> GetAllAsync(string userId)
        {
            var bag = GetBag(userId);
            return Task.FromResult(bag.OrderByDescending(n => n.CreatedAtUtc).AsEnumerable());
        }

        public Task MarkAsReadAsync(string userId, Guid notificationId)
        {
            var bag = GetBag(userId);
            var item = bag.FirstOrDefault(n => n.Id == notificationId);
            if (item != null) item.IsRead = true;
            return Task.CompletedTask;
        }

        // Demo seeding - only for first time a user is requested
        private ConcurrentBag<Notification> GetBag(string userId)
        {
            return _store.GetOrAdd(userId, id =>
            {
                var b = new ConcurrentBag<Notification>();

                // Updated seed links to match real controllers/views in your project.
                b.Add(new Notification
                {
                    UserId = id,
                    Type = NotificationType.Announcement,
                    Title = "New Announcement",
                    Message = "Midterm schedule published.",
                    Link = "/Portal/StudentPortal",      // PortalController.StudentPortal
                    CreatedAtUtc = DateTime.UtcNow.AddHours(-4),
                    IsRead = false
                });

                b.Add(new Notification
                {
                    UserId = id,
                    Type = NotificationType.Connection,
                    Title = "Tutor Connected",
                    Message = "Your tutor has accepted your request.",
                    Link = "/Portal/StudentPortal",      // safe fallback
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
                    IsRead = false
                });

                b.Add(new Notification
                {
                    UserId = id,
                    Type = NotificationType.Message,
                    Title = "New Message",
                    Message = "You have a new message in your inbox.",
                    Link = "/Portal/StudentPortal",      // messaging UI lives on portal pages (widget)
                    CreatedAtUtc = DateTime.UtcNow.AddHours(-2),
                    IsRead = false
                });

                b.Add(new Notification
                {
                    UserId = id,
                    Type = NotificationType.UpcomingQuiz,
                    Title = "Quiz Tomorrow",
                    Message = "Quiz: JavaScript Basics starts within 24 hours.",
                    Link = "/Portal/QuizPortal",         // PortalController.QuizPortal
                    CreatedAtUtc = DateTime.UtcNow.AddHours(-3),
                    IsRead = false
                });

                b.Add(new Notification
                {
                    UserId = id,
                    Type = NotificationType.IncompleteQuiz,
                    Title = "Incomplete Quiz",
                    Message = "You have an available quiz you haven't started yet.",
                    Link = "/Portal/QuizPortal",         // PortalController.QuizPortal
                    CreatedAtUtc = DateTime.UtcNow.AddHours(-10),
                    IsRead = false
                });

                return b;
            });
        }
    }
}