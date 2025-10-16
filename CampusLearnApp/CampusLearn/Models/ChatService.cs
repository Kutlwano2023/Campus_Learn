using CampusLearn.Data;
using CampusLearn.Models;
using CampusLearn.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace CampusLearn.Services
{
    public class ChatService : IChatService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ChatService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ChatService(AppDbContext context, ILogger<ChatService> logger,
                          HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<ChatResponse> ProcessMessageAsync(ChatRequest request, string userId)
        {
            try
            {
                // Get user data for context
                var user = await _context.Users.FindAsync(userId);
                if (user == null) throw new InvalidOperationException("User not found");

                // Get or create conversation
                var conversation = await GetOrCreateConversationAsync(request, userId);

                // Save user message to database
                var userMessage = new ChatMessage
                {
                    MessageId = Guid.NewGuid(),
                    ConversationId = conversation.ConversationId,
                    UserId = userId,
                    MessageText = request.Message,
                    IsUserMessage = true,
                    MessageType = "Text",
                    CreatedAt = DateTime.UtcNow
                };
                _context.ChatMessages.Add(userMessage);
                await _context.SaveChangesAsync();

                // Call Chatbase Api Here
                var chatBaseResponse = await CallChatBaseAPI(request.Message, user, conversation);

                // Save AI response to database
                var aiMessage = new ChatMessage
                {
                    MessageId = Guid.NewGuid(),
                    ConversationId = conversation.ConversationId,
                    UserId = null,
                    MessageText = chatBaseResponse.Response,
                    IsUserMessage = false,
                    MessageType = "Text",
                    CreatedAt = DateTime.UtcNow,
                    Metadata = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        SuggestedActions = chatBaseResponse.SuggestedActions,
                        Source = "ChatBase"
                    })
                };
                _context.ChatMessages.Add(aiMessage);

                // Update conversation
                conversation.UpdatedAt = DateTime.UtcNow;
                if (string.IsNullOrEmpty(conversation.Title) || conversation.Title == "New Conversation")
                {
                    conversation.Title = await GenerateConversationTitleAsync(request.Message);
                }
                await _context.SaveChangesAsync();

                return chatBaseResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat message");
                throw;
            }
        }

        private async Task<ChatResponse> CallChatBaseAPI(string userMessage, Users user, ChatConversation conversation)
        {
            try
            {
                var chatBaseConfig = _configuration.GetSection("ChatBase");
                var apiKey = chatBaseConfig["ApiKey"];
                var chatbotId = chatBaseConfig["ChatbotId"];

                // Prepare request to ChatBase API
                var requestBody = new
                {
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = userMessage
                        }
                    },
                    chatId = conversation.ConversationId.ToString(),
                    stream = false,
                    userId = user.Id,
                    userData = new
                    {
                        fullName = user.FullName,
                        role = user.Role,
                        email = user.Email
                    }
                };

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"https://www.chatbase.co/api/v1/chat"),
                    Headers =
                    {
                        { "Authorization", $"Bearer {apiKey}" },
                    },
                    Content = new StringContent(
                        System.Text.Json.JsonSerializer.Serialize(requestBody),
                        Encoding.UTF8,
                        "application/json")
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var chatBaseResult = System.Text.Json.JsonSerializer.Deserialize<ChatBaseResponse>(responseContent);

                return new ChatResponse
                {
                    MessageId = Guid.NewGuid(),
                    Response = chatBaseResult.Message,
                    ConversationId = conversation.ConversationId,
                    ConversationTitle = conversation.Title,
                    Timestamp = DateTime.UtcNow,
                    SuggestedActions = ExtractSuggestedActions(chatBaseResult)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling ChatBase API");
                // Fallback to local responses if ChatBase fails
                return GenerateFallbackResponse(userMessage, user, conversation);
            }
        }

        // ChatBase response model
        private class ChatBaseResponse
        {
            public string Message { get; set; }
            public object Data { get; set; }
        }

        private List<QuickAction> ExtractSuggestedActions(ChatBaseResponse response)
        {
            // Extract suggested actions from ChatBase response
            // This depends on how ChatBase returns suggested actions
            return new List<QuickAction>
            {
                new QuickAction { Text = "Browse Resources", Action = "navigate:/resources", Type = "button" },
                new QuickAction { Text = "Get Help", Action = "contact-support", Type = "button" }
            };
        }

        private ChatResponse GenerateFallbackResponse(string message, Users user, ChatConversation conversation)
        {
            // Your existing fallback logic
            var response = GenerateContextualResponse(message, user.Role, "General");
            var actions = GetSuggestedActions(message, user.Role);

            return new ChatResponse
            {
                MessageId = Guid.NewGuid(),
                Response = response,
                ConversationId = conversation.ConversationId,
                ConversationTitle = conversation.Title,
                Timestamp = DateTime.UtcNow,
                SuggestedActions = actions
            };
        }

        // Existing methods from your original ChatService
        private async Task<ChatConversation> GetOrCreateConversationAsync(ChatRequest request, string userId)
        {
            if (request.ConversationId.HasValue)
            {
                var existingConversation = await _context.ChatConversations
                    .FirstOrDefaultAsync(c => c.ConversationId == request.ConversationId && c.UserId == userId);

                if (existingConversation != null)
                    return existingConversation;
            }

            var conversation = new ChatConversation
            {
                ConversationId = Guid.NewGuid(),
                UserId = userId,
                Title = "New Conversation",
                ConversationType = request.ConversationType,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ChatConversations.Add(conversation);
            return conversation;
        }

        private string GenerateContextualResponse(string message, string userRole, string conversationType)
        {
            var lowerMessage = message.ToLower();

            if (lowerMessage.Contains("assignment") || lowerMessage.Contains("homework"))
            {
                return userRole == "Tutor"
                    ? "I can help you with assignment management. You can create new assignments, review submissions, or set deadlines through the Tutor Portal."
                    : "For assignment submissions, please use the Student Portal. You can upload files and track submission status there.";
            }

            if (lowerMessage.Contains("resource") || lowerMessage.Contains("material"))
            {
                return "You can access learning materials through the Resources section. We have study guides, research papers, and notes available in various formats (PDF, DOCX).";
            }

            if (lowerMessage.Contains("grade") || lowerMessage.Contains("score"))
            {
                return userRole == "Tutor"
                    ? "You can manage and review student grades through the Gradebook in your Tutor Dashboard."
                    : "Your grades and assessment results are available in the Student Portal under 'My Progress'.";
            }

            if (lowerMessage.Contains("login") || lowerMessage.Contains("password"))
            {
                return "For login issues, please try resetting your password or contact campus IT support.";
            }

            return $"Thank you for your message. As a {userRole}, I'm here to help you with {conversationType.ToLower()} inquiries. How can I assist you further with CampusLearn?";
        }

        private List<QuickAction> GetSuggestedActions(string message, string userRole)
        {
            var lowerMessage = message.ToLower();
            var actions = new List<QuickAction>();

            if (lowerMessage.Contains("help") || lowerMessage.Contains("support"))
            {
                actions.AddRange(new[]
                {
                    new QuickAction { Text = "View Resources", Action = "navigate:/resources", Type = "button" },
                    new QuickAction { Text = "Contact Support", Action = "contact-support", Type = "button" }
                });
            }

            // Add default actions
            actions.Add(new QuickAction { Text = "Browse Resources", Action = "navigate:/resources", Type = "button" });

            return actions;
        }

        public async Task<List<ChatConversation>> GetUserConversationsAsync(string userId)
        {
            return await _context.ChatConversations
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.UpdatedAt)
                .ToListAsync();
        }

        public async Task<ChatConversation> GetConversationWithMessagesAsync(Guid conversationId)
        {
            return await _context.ChatConversations
                .Include(c => c.Messages.OrderBy(m => m.CreatedAt))
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);
        }

        public async Task DeleteConversationAsync(Guid conversationId, string userId)
        {
            var conversation = await _context.ChatConversations
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId && c.UserId == userId);

            if (conversation != null)
            {
                _context.ChatConversations.Remove(conversation);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<string> GenerateConversationTitleAsync(string firstMessage)
        {
            try
            {
                var text = firstMessage.Trim();
                return text.Length > 50 ? text[..47] + "..." : text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating conversation title");
                return "CampusLearn Conversation";
            }
        }
    }
}