using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CampusLearn.Models;

namespace CampusLearn.Services
{
    public interface INotificationService
    {
        Task<int> GetUnreadCountAsync(string userId);
        Task<IEnumerable<Notification>> GetRecentAsync(string userId, int limit = 20);
        Task MarkAsReadAsync(string userId, Guid notificationId);
        Task<IEnumerable<Notification>> GetAllAsync(string userId);
    }
}