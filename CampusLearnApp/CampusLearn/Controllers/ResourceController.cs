using Microsoft.AspNetCore.Mvc;
using CampusLearn.Models;
using System.Collections.Generic;

namespace CampusLearn.Controllers
{
    public class ResourceController : Controller
    {
        public IActionResult Index()
        {
            var resources = new List<Resource>
            {
                new Resource { Title = "React Component Best Practices", Author = "Sarah Chen", Type = "Study Guide", FileType = "PDF", FileSizeMB = 2.4, UploadedAgo = "3 days ago", Downloads = 234, FileUrl = "#" },
                new Resource { Title = "JavaScript ES6 Cheat Sheet", Author = "Mike Rodriguez", Type = "Notes", FileType = "PDF", FileSizeMB = 1.8, UploadedAgo = "1 week ago", Downloads = 456, FileUrl = "#" },
                new Resource { Title = "Database Design Patterns", Author = "Dr. Lisa Wang", Type = "Research Paper", FileType = "DOCX", FileSizeMB = 3.2, UploadedAgo = "5 days ago", Downloads = 189, FileUrl = "#" },
                new Resource { Title = "CSS Grid Layout Guide", Author = "Emma Wilson", Type = "Study Guide", FileType = "PDF", FileSizeMB = 4.1, UploadedAgo = "2 days ago", Downloads = 312, FileUrl = "#" }
            };

            return View(resources);
        }
    }
}
