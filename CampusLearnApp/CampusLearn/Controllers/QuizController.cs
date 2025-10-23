using Microsoft.AspNetCore.Mvc;
using CampusLearn.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CampusLearn.Controllers
{
    public class QuizController : Controller
    {
        public IActionResult Index()
        {
            var model = new QuizViewModel
            {
                UserRole = User.IsInRole("Tutor") ? "Tutor" : "Student",
                AvailableQuizzes = GetAvailableQuizzes(),
                RecentResults = GetRecentResults(),
                Categories = GetQuizCategories(),
                RemainingQuizzes = GetRemainingQuizzes(),
                Analytics = GetQuizAnalytics()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult StartQuiz(string quizId)
        {
            // In real application, you would start a quiz session
            TempData["SuccessMessage"] = "Quiz started successfully!";
            return RedirectToAction("TakeQuiz", new { id = quizId });
        }

        [HttpPost]
        public IActionResult CreateQuiz(string title, string description, string subject, string difficulty)
        {
            // In real application, you would save to database
            TempData["SuccessMessage"] = $"Quiz '{title}' created successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult TakeQuiz(string id)
        {
            // This would be the actual quiz taking page
            // For now, redirect back with message
            TempData["InfoMessage"] = "Quiz taking functionality coming soon!";
            return RedirectToAction("Index");
        }

        private List<Quiz> GetAvailableQuizzes()
        {
            return new List<Quiz>
            {
                new Quiz
                {
                    Title = "React Hooks Fundamentals",
                    Description = "Understand useState, useEffect, and custom hooks",
                    Subject = "react",
                    Difficulty = "Intermediate",
                    QuestionCount = 15,
                    Duration = 20,
                    AverageScore = 78,
                    Author = "Sarah Chen",
                    AuthorInitials = "SC",
                    Rating = 4.8,
                    Attempts = 234,
                    Tags = new List<string> { "react", "intermediate" }
                },
                new Quiz
                {
                    Title = "JavaScript Array Methods",
                    Description = "Master map, filter, reduce and more",
                    Subject = "javascript",
                    Difficulty = "Beginner",
                    QuestionCount = 12,
                    Duration = 15,
                    AverageScore = 82,
                    Author = "Mike Rodriguez",
                    AuthorInitials = "MR",
                    Rating = 4.9,
                    Attempts = 456,
                    Tags = new List<string> { "javascript", "beginner" }
                },
                new Quiz
                {
                    Title = "CSS Flexbox & Grid",
                    Description = "Advanced layout techniques and responsive patterns",
                    Subject = "css",
                    Difficulty = "Advanced",
                    QuestionCount = 20,
                    Duration = 25,
                    AverageScore = 71,
                    Author = "Emma Wilson",
                    AuthorInitials = "EW",
                    Rating = 4.3,
                    Attempts = 189,
                    Tags = new List<string> { "css", "advanced" }
                }
            };
        }

        private List<QuizResult> GetRecentResults()
        {
            return new List<QuizResult>
            {
                new QuizResult
                {
                    QuizTitle = "React Hooks Fundamentals",
                    Score = 85,
                    CorrectAnswers = 13,
                    TotalQuestions = 15,
                    TimeSpent = "18:30",
                    Status = "passed",
                    AttemptNumber = 2,
                    DateTaken = DateTime.Now.AddDays(-2)
                },
                new QuizResult
                {
                    QuizTitle = "JavaScript Array Methods",
                    Score = 92,
                    CorrectAnswers = 11,
                    TotalQuestions = 12,
                    TimeSpent = "12:45",
                    Status = "passed",
                    AttemptNumber = 1,
                    DateTaken = DateTime.Now.AddDays(-3)
                },
                new QuizResult
                {
                    QuizTitle = "Database Design Quiz",
                    Score = 68,
                    CorrectAnswers = 12,
                    TotalQuestions = 18,
                    TimeSpent = "22:15",
                    Status = "review",
                    AttemptNumber = 1,
                    DateTaken = DateTime.Now.AddDays(-5)
                }
            };
        }

        private List<QuizCategory> GetQuizCategories()
        {
            return new List<QuizCategory>
            {
                new QuizCategory { Name = "React", Icon = "⚛️", QuizCount = 24 },
                new QuizCategory { Name = "JavaScript", Icon = "🟨", QuizCount = 18 },
                new QuizCategory { Name = "CSS", Icon = "🎨", QuizCount = 15 },
                new QuizCategory { Name = "Backend", Icon = "🛠️", QuizCount = 12 },
                new QuizCategory { Name = "Database", Icon = "🗃️", QuizCount = 9 },
                new QuizCategory { Name = "General", Icon = "🧩", QuizCount = 6 }
            };
        }

        private List<Quiz> GetRemainingQuizzes()
        {
            return new List<Quiz>
            {
                new Quiz
                {
                    Title = "TypeScript Fundamentals",
                    Description = "Learn TypeScript basics and type system",
                    Subject = "typescript",
                    Difficulty = "Intermediate",
                    QuestionCount = 18,
                    Duration = 22,
                    AverageScore = 79
                },
                new Quiz
                {
                    Title = "Docker Containerization Basics",
                    Description = "Understand Docker containers and images",
                    Subject = "devops",
                    Difficulty = "Beginner",
                    QuestionCount = 20,
                    Duration = 25,
                    AverageScore = 85
                },
                new Quiz
                {
                    Title = "GraphQL Query Language",
                    Description = "Master GraphQL queries and mutations",
                    Subject = "api",
                    Difficulty = "Advanced",
                    QuestionCount = 24,
                    Duration = 30,
                    AverageScore = 68
                }
            };
        }

        private QuizAnalytics GetQuizAnalytics()
        {
            return new QuizAnalytics
            {
                QuizzesTaken = 24,
                AverageScore = 82,
                StudyTime = "18h",
                Streak = 12,
                Trends = new List<PerformanceTrend>
                {
                    new PerformanceTrend { Period = "Week 1", Score = 65 },
                    new PerformanceTrend { Period = "Week 2", Score = 72 },
                    new PerformanceTrend { Period = "Week 3", Score = 78 },
                    new PerformanceTrend { Period = "Week 4", Score = 85 }
                },
                SubjectPerformance = new List<SubjectPerformance>
                {
                    new SubjectPerformance { Subject = "React", Score = 88 },
                    new SubjectPerformance { Subject = "JavaScript", Score = 82 },
                    new SubjectPerformance { Subject = "CSS", Score = 76 },
                    new SubjectPerformance { Subject = "Backend", Score = 71 }
                }
            };
        }
    }
}