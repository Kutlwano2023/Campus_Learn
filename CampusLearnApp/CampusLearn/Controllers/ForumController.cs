// Controllers/ForumController.cs
using Microsoft.AspNetCore.Mvc;
using CampusLearn.Models;
using CampusLearn.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CamusLearn.Controllers
{
    public class ForumController : Controller
    {
        public IActionResult Index()
        {
            var model = new ForumViewModel
            {
                UserRole = User.IsInRole("Tutor") ? "Tutor" : "Student",
                Topics = GetRecentTopics(),
                PopularTopics = GetPopularTopics(),
                UnansweredTopics = GetUnansweredTopics(),
                Stats = new ForumStats
                {
                    TotalTopics = 1234,
                    TotalPosts = 8965,
                    ActiveUsers = 156
                }
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult CreateTopic(string title, string content, string category)
        {
            // In a real application, you would save to database
            // For now, we'll just redirect back with a success message
            TempData["SuccessMessage"] = $"Topic '{title}' created successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddReply(string topicId, string replyContent)
        {
            // In a real application, you would save the reply to database
            TempData["SuccessMessage"] = "Reply posted successfully!";
            return RedirectToAction("Index");
        }

        private List<ForumTopic> GetRecentTopics()
        {
            return new List<ForumTopic>
            {
                new ForumTopic
                {
                    Title = "Best practices for React component optimization?",
                    Author = "Sarah Chen",
                    AuthorInitials = "SC",
                    Category = "react",
                    PostedTime = "2 hours ago",
                    ReplyCount = 24,
                    LikeCount = 18,
                    IsFeatured = true,
                    IsPinned = true,
                    Tags = new List<string> { "React", "Performance", "Optimization" }
                },
                new ForumTopic
                {
                    Title = "Help with JavaScript async/await concepts",
                    Author = "Mike Rodriguez",
                    AuthorInitials = "MR",
                    Category = "js",
                    PostedTime = "4 hours ago",
                    ReplyCount = 15,
                    LikeCount = 12,
                    IsFeatured = false,
                    Tags = new List<string> { "JavaScript", "Async", "Promises" }
                },
                new ForumTopic
                {
                    Title = "CSS Grid vs Flexbox - When to use which?",
                    Author = "Emma Wilson",
                    AuthorInitials = "EW",
                    Category = "css",
                    PostedTime = "1 day ago",
                    ReplyCount = 31,
                    LikeCount = 27,
                    IsFeatured = false,
                    Tags = new List<string> { "CSS", "Layout", "Flexbox" }
                },
                new ForumTopic
                {
                    Title = "Database design patterns for beginners",
                    Author = "David Kim",
                    AuthorInitials = "DK",
                    Category = "db",
                    PostedTime = "2 days ago",
                    ReplyCount = 19,
                    LikeCount = 22,
                    IsFeatured = false,
                    Tags = new List<string> { "Database", "Design", "SQL" }
                },
                new ForumTopic
                {
                    Title = "Understanding MVC pattern in ASP.NET Core",
                    Author = "Alex Thompson",
                    AuthorInitials = "AT",
                    Category = "backend",
                    PostedTime = "3 days ago",
                    ReplyCount = 8,
                    LikeCount = 14,
                    IsFeatured = true,
                    Tags = new List<string> { "ASP.NET", "MVC", "Backend" }
                }
            };
        }

        private List<ForumTopic> GetPopularTopics()
        {
            return new List<ForumTopic>
            {
                new ForumTopic
                {
                    Title = "Full-stack developer roadmap 2024",
                    Author = "Lisa Zhang",
                    AuthorInitials = "LZ",
                    Category = "career",
                    PostedTime = "1 hour ago",
                    ReplyCount = 45,
                    LikeCount = 89,
                    IsFeatured = true,
                    Tags = new List<string> { "Career", "Roadmap", "Full-stack" }
                },
                new ForumTopic
                {
                    Title = "TypeScript vs JavaScript - Migration guide",
                    Author = "Tom Bradley",
                    AuthorInitials = "TB",
                    Category = "ts",
                    PostedTime = "3 hours ago",
                    ReplyCount = 38,
                    LikeCount = 56,
                    IsFeatured = false,
                    Tags = new List<string> { "TypeScript", "JavaScript", "Migration" }
                },
                new ForumTopic
                {
                    Title = "React Hooks deep dive - useEffect pitfalls",
                    Author = "Maria Garcia",
                    AuthorInitials = "MG",
                    Category = "react",
                    PostedTime = "5 hours ago",
                    ReplyCount = 42,
                    LikeCount = 67,
                    IsFeatured = false,
                    Tags = new List<string> { "React", "Hooks", "useEffect" }
                }
            };
        }

        private List<ForumTopic> GetUnansweredTopics()
        {
            return new List<ForumTopic>
            {
                new ForumTopic
                {
                    Title = "How do I fix CORS errors when calling the API?",
                    Author = "Alice Lee",
                    AuthorInitials = "AL",
                    Category = "backend",
                    PostedTime = "3 hours ago",
                    ReplyCount = 0,
                    LikeCount = 2,
                    IsFeatured = false,
                    Tags = new List<string> { "API", "CORS" }
                },
                new ForumTopic
                {
                    Title = "Why is my CSS Grid not centering the card?",
                    Author = "Ravi Joshi",
                    AuthorInitials = "RJ",
                    Category = "css",
                    PostedTime = "5 hours ago",
                    ReplyCount = 0,
                    LikeCount = 1,
                    IsFeatured = false,
                    Tags = new List<string> { "CSS Grid", "Layout" }
                },
                new ForumTopic
                {
                    Title = "Is there a recommended way to structure redux stores?",
                    Author = "Sofia Martinez",
                    AuthorInitials = "SM",
                    Category = "react",
                    PostedTime = "Yesterday",
                    ReplyCount = 0,
                    LikeCount = 3,
                    IsFeatured = false,
                    Tags = new List<string> { "Redux", "State" }
                },
                new ForumTopic
                {
                    Title = "Database migration strategies for zero downtime",
                    Author = "James Wilson",
                    AuthorInitials = "JW",
                    Category = "db",
                    PostedTime = "Yesterday",
                    ReplyCount = 0,
                    LikeCount = 5,
                    IsFeatured = false,
                    Tags = new List<string> { "Database", "Migration", "DevOps" }
                }
            };
        }
    }
}