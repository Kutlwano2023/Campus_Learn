// Controllers/ResourcesController.cs
using CampusLearn.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CampusLearn.Controllers
{
    public class ResourcesController : Controller
    {
        public IActionResult Resource()  // Changed from Index to Resource
        {
            var model = new ResourceViewModel
            {
                UserRole = User.IsInRole("TUTOR") ? "Tutor" : "Student",  // Fixed role check to match other controllers
                NotesAndDocuments = GetNotesAndDocuments(),
                StudyGuides = GetStudyGuides(),
                ResearchPapers = GetResearchPapers(),
                CourseDownloads = GetCourseDownloads()
            };

            return View("Resource");  // Explicitly specify the view name
        }

        [HttpPost]
        public IActionResult DownloadResource(string resourceId)
        {
            // In real application, you would track downloads and serve the file
            var resource = GetResourceById(resourceId);
            if (resource != null)
            {
                TempData["SuccessMessage"] = $"Downloading '{resource.Title}'...";
                // Track download count in database
            }
            return RedirectToAction("Resource");  // Changed from Index to Resource
        }

        [HttpPost]
        public IActionResult UploadResource(string title, string description, string type, string fileType)
        {
            if (User.IsInRole("TUTOR"))  // Fixed role check to match other controllers
            {
                // In real application, you would save the file and resource metadata
                TempData["SuccessMessage"] = $"Resource '{title}' uploaded successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Only tutors can upload resources.";
            }
            return RedirectToAction("Resource");  // Changed from Index to Resource
        }

        private List<Resource> GetNotesAndDocuments()
        {
            return new List<Resource>
            {
                new Resource
                {
                    Title = "React Component Best Practices",
                    Type = "Study Guide",
                    FileType = "PDF",
                    Author = "Sarah Chen",
                    CreatedAt = DateTime.Now.AddDays(-3),
                    FileSize = 2.4,
                    DownloadCount = 234,
                    Icon = "📄",
                    Tags = new List<string> { "React", "Components", "Best Practices" }
                },
                new Resource
                {
                    Title = "Database Design Patterns",
                    Type = "Research Paper",
                    FileType = "DOCX",
                    Author = "Dr. Lisa Wang",
                    CreatedAt = DateTime.Now.AddDays(-5),
                    FileSize = 3.2,
                    DownloadCount = 189,
                    Icon = "📝",
                    Tags = new List<string> { "Database", "Design", "Patterns" }
                },
                new Resource
                {
                    Title = "React Hooks Explanation",
                    Type = "Notes",
                    FileType = "Audio",
                    Author = "James Liu",
                    CreatedAt = DateTime.Now.AddDays(-4),
                    FileSize = 12.5,
                    DownloadCount = 178,
                    Icon = "🎧",
                    Tags = new List<string> { "React", "Hooks", "Audio" }
                },
                new Resource
                {
                    Title = "JavaScript ES6 Cheat Sheet",
                    Type = "Notes",
                    FileType = "PDF",
                    Author = "Mike Rodriguez",
                    CreatedAt = DateTime.Now.AddDays(-7),
                    FileSize = 1.8,
                    DownloadCount = 456,
                    Icon = "🧾",
                    Tags = new List<string> { "JavaScript", "ES6", "Cheat Sheet" }
                },
                new Resource
                {
                    Title = "CSS Grid Layout Guide",
                    Type = "Study Guide",
                    FileType = "PDF",
                    Author = "Emma Wilson",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    FileSize = 4.1,
                    DownloadCount = 312,
                    Icon = "📘",
                    Tags = new List<string> { "CSS", "Grid", "Layout" }
                }
            };
        }

        private List<Resource> GetStudyGuides()
        {
            return new List<Resource>
            {
                new Resource
                {
                    Title = "React Component Best Practices",
                    Type = "Study Guide",
                    FileType = "PDF",
                    Author = "Sarah Chen",
                    CreatedAt = DateTime.Now.AddDays(-3),
                    FileSize = 2.4,
                    DownloadCount = 234,
                    Icon = "📄"
                },
                new Resource
                {
                    Title = "CSS Grid Layout Guide",
                    Type = "Study Guide",
                    FileType = "PDF",
                    Author = "Emma Wilson",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    FileSize = 4.1,
                    DownloadCount = 312,
                    Icon = "📘"
                }
            };
        }

        private List<Resource> GetResearchPapers()
        {
            return new List<Resource>
            {
                new Resource
                {
                    Title = "Database Design Patterns",
                    Description = "Comprehensive research on modern database design patterns and their implementation in real-world applications.",
                    Type = "Research Paper",
                    FileType = "DOCX",
                    Author = "Dr. Lisa Wang",
                    CreatedAt = DateTime.Now.AddDays(-5),
                    FileSize = 3.2,
                    DownloadCount = 189,
                    Icon = "📝"
                }
            };
        }

        private List<ResourcePack> GetCourseDownloads()
        {
            return new List<ResourcePack>
            {
                new ResourcePack
                {
                    Title = "React Complete Course Materials",
                    Description = "Complete set of React course materials including examples and exercises",
                    FileCount = 24,
                    TotalSize = 156,
                    Category = "Course Pack",
                    Icon = "⚛️"
                },
                new ResourcePack
                {
                    Title = "JavaScript Reference Guide",
                    Description = "Comprehensive JavaScript reference with examples and best practices",
                    FileCount = 12,
                    TotalSize = 89,
                    Category = "Reference",
                    Icon = "🟨"
                },
                new ResourcePack
                {
                    Title = "Web Development Templates",
                    Description = "Collection of web development templates and starter kits",
                    FileCount = 36,
                    TotalSize = 234,
                    Category = "Templates",
                    Icon = "🌐"
                }
            };
        }

        private Resource GetResourceById(string id)
        {
            // This would fetch from database in real application
            var allResources = GetNotesAndDocuments()
                .Concat(GetStudyGuides())
                .Concat(GetResearchPapers())
                .ToList();

            return allResources.FirstOrDefault(r => r.Id == id);
        }
    }
}
