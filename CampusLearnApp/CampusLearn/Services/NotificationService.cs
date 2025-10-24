using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CampusLearn.Models;

namespace CampusLearn.Services
{
    // Simple in-memory demo service. Replace with DB-backed calls later.
    // This class now includes a simple TCP socket listener running on a background thread.
    // Incoming connections should send a single JSON notification per line. Each JSON object
    // is deserialized into a Notification and stored in the in-memory store.
    public class NotificationService : INotificationService, IDisposable
    {
        private static readonly ConcurrentDictionary<string, ConcurrentBag<Notification>> _store =
            new ConcurrentDictionary<string, ConcurrentBag<Notification>>();

        // Socket / threading fields
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly Thread _listenerThread;
        private TcpListener? _listener;
        private readonly int _port;

        // JSON options allow enum-by-name and case-insensitive property names
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        // Default port chosen for demo; you can change or make configurable in Program.cs
        public NotificationService(int port = 5500)
        {
            _port = port;

            // Start a dedicated thread to accept socket connections.
            // Using a dedicated thread demonstrates explicit threading model; each connection
            // is handed off to a Task to avoid blocking the accept loop.
            _listenerThread = new Thread(() => ListenerLoop(_cts.Token))
            {
                IsBackground = true,
                Name = "NotificationSocketListener"
            };

            _listenerThread.Start();
        }

        // Public API methods (unchanged behavior)
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

        // Dispose - stop listener cleanly
        public void Dispose()
        {
            StopSocketListener();
            _cts.Dispose();
        }

        // Explicit stop method if you prefer to call manually
        public void StopSocketListener()
        {
            try
            {
                _cts.Cancel();
                try
                {
                    _listener?.Stop();
                }
                catch { /* swallow - best effort */ }

                if (_listenerThread.IsAlive)
                {
                    // Wait a short time for the thread to exit gracefully
                    if (!_listenerThread.Join(TimeSpan.FromSeconds(2)))
                    {
                        // If it doesn't stop, it's still background thread so process shutdown will clean it up
                    }
                }
            }
            catch { /* swallow - disposal should not throw */ }
        }

        // Listener loop runs on its own thread.
        // Accepts TCP clients and processes each client on a Task.
        private void ListenerLoop(CancellationToken token)
        {
            try
            {
                _listener = new TcpListener(IPAddress.Loopback, _port);
                _listener.Start();

                // Accept loop
                while (!token.IsCancellationRequested)
                {
                    // Use Pending() to avoid blocking indefinitely on AcceptTcpClient when cancellation requested.
                    if (_listener.Pending())
                    {
                        TcpClient client = _listener.AcceptTcpClient();
                        // Handle client on thread-pool to keep accept loop responsive.
                        _ = Task.Run(() => HandleClientAsync(client, token), token);
                    }
                    else
                    {
                        // Small sleep to avoid busy-wait; responsive enough for demo scenarios
                        Thread.Sleep(100);
                    }
                }
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted || ex.SocketErrorCode == SocketError.OperationAborted)
            {
                // Expected when stopping listener; swallow silently
            }
            catch (Exception ex)
            {
                // In production, log this. For demo, swallow after a minor delay to avoid tight exception loops.
                try { Thread.Sleep(500); } catch { }
            }
            finally
            {
                try { _listener?.Stop(); } catch { }
            }
        }

        // Accepts a client, reads UTF-8 text lines, each line expected to be JSON representing a Notification.
        // Any valid notification will be added to the in-memory store for the indicated UserId.
        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            using (client)
            {
                try
                {
                    var stream = client.GetStream();
                    var buffer = new byte[4096];
                    var sb = new StringBuilder();

                    while (!token.IsCancellationRequested)
                    {
                        if (!stream.DataAvailable)
                        {
                            // Wait a short time rather than busy loop
                            await Task.Delay(50, token).ConfigureAwait(false);
                            if (!client.Connected) break;
                            continue;
                        }

                        int read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), token).ConfigureAwait(false);
                        if (read == 0) break; // client closed connection

                        sb.Append(Encoding.UTF8.GetString(buffer, 0, read));

                        // Process whole-lines (newline delimited). Clients should send newline (\n) after each JSON object.
                        string content = sb.ToString();
                        int newlineIdx;
                        while ((newlineIdx = content.IndexOf('\n')) >= 0)
                        {
                            string line = content.Substring(0, newlineIdx).Trim();
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                TryProcessJsonLine(line);
                            }
                            content = content.Substring(newlineIdx + 1);
                        }

                        // Keep leftover partial content
                        sb.Clear();
                        sb.Append(content);
                    }
                }
                catch (OperationCanceledException) { /* cancellation - exit quietly */ }
                catch { /* in demo swallow errors per-connection */ }
            }
        }

        // Try to deserialize the JSON line and add to store.
        private void TryProcessJsonLine(string jsonLine)
        {
            try
            {
                var notification = JsonSerializer.Deserialize<Notification>(jsonLine, _jsonOptions);
                if (notification == null) return;

                // Ensure required fields are set
                if (notification.Id == Guid.Empty) notification.Id = Guid.NewGuid();
                if (notification.CreatedAtUtc == default) notification.CreatedAtUtc = DateTime.UtcNow;
                if (string.IsNullOrWhiteSpace(notification.UserId)) return; // cannot store without UserId

                var bag = GetBag(notification.UserId);
                bag.Add(notification);
            }
            catch
            {
                // Ignore malformed JSON in demo. In production, log and/or send error back to client.
            }
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