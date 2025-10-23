using CampusLearn.Models;
using CampusLearn.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CampusLearn.Controllers
{
    [Authorize]
    public class PortalController : Controller
    {
        private readonly ILogger<PortalController> _logger;

        public PortalController(ILogger<PortalController> logger)
        {
            _logger = logger;
        }

        public IActionResult Dashboard()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";
            _logger.LogInformation("User {UserName} accessed dashboard", User.Identity.Name);
            return View();
        }

        [Authorize(Roles = "STUDENT")]
        public IActionResult StudentPortal()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = "Student";
            _logger.LogInformation("Student {UserName} accessed student portal", User.Identity.Name);
            return View();
        }

        [Authorize(Roles = "TUTOR")]
        public IActionResult TutorPortal()
        {
            _logger.LogInformation("Tutor {UserName} requested TutorPortal; redirecting to TutorController", User.Identity.Name);

            // FIX: Redirect to the TutorController's TutorPortal action
            return RedirectToAction("TutorPortal", "Tutor");
        }

        [HttpGet]
        public async Task<IActionResult> Course(int id)
        {
            try
            {
                // Implement course viewing logic with error handling
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid course ID: {CourseId}", id);
                    return BadRequest("Invalid course ID");
                }

                // Add course retrieval logic here
                ViewBag.CourseId = id;
                ViewBag.UserName = User.Identity.Name;
                ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";

                _logger.LogInformation("User {UserName} accessed course {CourseId}", User.Identity.Name, id);
                return View();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error accessing course {CourseId} for user {UserName}", id, User.Identity.Name);
                return StatusCode(500, "An error occurred while accessing the course");
            }
        }

        [HttpGet]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> TakeAssessment(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid assessment ID");
                }

                // Implement assessment taking logic
                ViewBag.AssessmentId = id;
                ViewBag.UserName = User.Identity.Name;

                _logger.LogInformation("Student {UserName} taking assessment {AssessmentId}", User.Identity.Name, id);
                return View();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error taking assessment {AssessmentId} for student {UserName}", id, User.Identity.Name);
                return StatusCode(500, "An error occurred while loading the assessment");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReviewAssessment(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid assessment ID");
                }

                // Implement assessment review logic
                ViewBag.AssessmentId = id;
                ViewBag.UserName = User.Identity.Name;
                ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";

                _logger.LogInformation("User {UserName} reviewing assessment {AssessmentId}", User.Identity.Name, id);
                return View();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error reviewing assessment {AssessmentId} for user {UserName}", id, User.Identity.Name);
                return StatusCode(500, "An error occurred while loading the assessment review");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> JoinGroup(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { success = false, message = "Invalid group ID" });
                }

                // Implement group joining logic with validation
                _logger.LogInformation("Student {UserName} joined group {GroupId}", User.Identity.Name, id);

                return Json(new { success = true, message = "Successfully joined group" });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error joining group {GroupId} for student {UserName}", id, User.Identity.Name);
                return Json(new { success = false, message = "An error occurred while joining the group" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> ConnectPartner(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { success = false, message = "Invalid partner ID" });
                }

                // Implement partner connection logic with validation
                _logger.LogInformation("Student {UserName} requested connection with partner {PartnerId}", User.Identity.Name, id);

                return Json(new { success = true, message = "Connection request sent" });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error connecting partner {PartnerId} for student {UserName}", id, User.Identity.Name);
                return Json(new { success = false, message = "An error occurred while sending connection request" });
            }
        }

        [HttpGet]
        public IActionResult ScheduleSession()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";
            return View();
        }

        [HttpGet]
        public IActionResult Resources()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";
            _logger.LogInformation("User {UserName} accessed resources", User.Identity.Name);
            return View();
        }

        [HttpGet]
        public IActionResult QuizPortal()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";

            // Create and pass the quiz model
            var model = new QuizViewModel
            {
                UserRole = User.IsInRole("TUTOR") ? "Tutor" : "Student",
                AvailableQuizzes = GetAvailableQuizzes(),
                RecentResults = GetRecentResults(),
                Categories = GetQuizCategories(),
                RemainingQuizzes = GetRemainingQuizzes(),
                Analytics = GetQuizAnalytics()
            };

            return View(model);
        }

        // Add these helper methods to PortalController
        private List<Quiz> GetAvailableQuizzes()
        {
            return new List<Quiz>
    {
        new Quiz
        {
            Id = "1",  // Change to string
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
            Id = "2",  // Change to string
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
        }
    };
        }

        private List<Quiz> GetRemainingQuizzes()
        {
            return new List<Quiz>
    {
        new Quiz
        {
            Id = "3",  // Change to string
            Title = "TypeScript Fundamentals",
            Description = "Learn TypeScript basics and type system",
            Subject = "typescript",
            Difficulty = "Intermediate",
            QuestionCount = 18,
            Duration = 22,
            AverageScore = 79
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
        }
    };
        }

        private List<QuizCategory> GetQuizCategories()
        {
            return new List<QuizCategory>
    {
        new QuizCategory { Name = "React", Icon = "⚛️", QuizCount = 24 },
        new QuizCategory { Name = "JavaScript", Icon = "🟨", QuizCount = 18 }
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
            new PerformanceTrend { Period = "Week 2", Score = 72 }
        },
                SubjectPerformance = new List<SubjectPerformance>
        {
            new SubjectPerformance { Subject = "React", Score = 88 },
            new SubjectPerformance { Subject = "JavaScript", Score = 82 }
        }
            };
        }

        [HttpGet]
        public IActionResult Tests()
        {
            // Fixed role check consistency
            ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";
            ViewBag.UserName = User.Identity.Name;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "STUDENT")]
        public async Task<IActionResult> TakeTest(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid test ID");
                }

                // Implement test taking logic
                ViewBag.TestId = id;
                ViewBag.UserName = User.Identity.Name;

                _logger.LogInformation("Student {UserName} taking test {TestId}", User.Identity.Name, id);
                return View();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error taking test {TestId} for student {UserName}", id, User.Identity.Name);
                return StatusCode(500, "An error occurred while loading the test");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReviewTest(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid test ID");
                }

                // Implement test review logic
                ViewBag.TestId = id;
                ViewBag.UserName = User.Identity.Name;
                ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";

                _logger.LogInformation("User {UserName} reviewing test {TestId}", User.Identity.Name, id);
                return View();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error reviewing test {TestId} for user {UserName}", id, User.Identity.Name);
                return StatusCode(500, "An error occurred while loading the test review");
            }
        }

        [HttpGet]
        public IActionResult Assignments()
        {
            var userRole = User.IsInRole("TUTOR") ? "Tutor" : "Student";
            ViewBag.Role = userRole;
            ViewBag.UserName = User.Identity.Name;
            return View();
        }

        [HttpGet]
        public IActionResult Forum()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Role = User.IsInRole("TUTOR") ? "Tutor" : "Student";

            // Create and pass the forum model
            var model = new ForumViewModel
            {
                UserRole = User.IsInRole("TUTOR") ? "Tutor" : "Student",
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
        }
    };
        }

        // Additional utility action for error handling
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}