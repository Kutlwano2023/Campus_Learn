using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CampusLearn.Data;
using CampusLearn.Models;
using Microsoft.EntityFrameworkCore;

namespace CampusLearn.Services
{
    public class ChatService : IChatService
    {
        private readonly AppDbContext _db;
        public ChatService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Conversation> GetOrCreateConversationAsync(string studentId, string tutorId)
        {
            var conv = await _db.Conversations
                .Include(c => c.Messages.OrderBy(m => m.SentAt))
                .FirstOrDefaultAsync(c => c.StudentId == studentId && c.TutorId == tutorId);

            if (conv != null) return conv;

            conv = new Conversation
            {
                StudentId = studentId,
                TutorId = tutorId,
                Title = $"Chat {studentId} ↔ {tutorId}",
                CreatedAt = DateTime.UtcNow
            };

            _db.Conversations.Add(conv);
            await _db.SaveChangesAsync();
            return conv;
        }

        public async Task<Conversation> GetConversationAsync(Guid conversationId)
        {
            return await _db.Conversations
                .Include(c => c.Messages.OrderBy(m => m.SentAt))
                .FirstOrDefaultAsync(c => c.Id == conversationId);
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(Guid conversationId, int page = 0, int pageSize = 50)
        {
            return await _db.ChatMessages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.SentAt)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<ChatMessage> AddMessageAsync(Guid conversationId, string senderId, string senderName, string text)
        {
            var message = new ChatMessage
            {
                ConversationId = conversationId,
                SenderId = senderId,
                SenderName = senderName,
                Text = text,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _db.ChatMessages.Add(message);

            var conv = await _db.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId);
            if (conv != null)
            {
                conv.LastUpdated = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return message;
        }
    }
}
