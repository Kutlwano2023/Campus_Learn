using CampusLearn.Models;
using CampusLearn.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CampusLearn.Controllers
{
    public class ResourceController : Controller
    {
        private readonly MongoService _mongoService;

        public ResourceController(MongoService mongoService)
        {
            _mongoService = mongoService;
        }

        public async Task<IActionResult> Index()
        {
            var resources = await _mongoService.Resources.Find(_ => true).ToListAsync();

            // If no resources in database, use sample data
            if (resources.Count == 0)
            {
                resources = new List<Resource>
                {
                    new Resource {
                        Title = "React Component Best Practices",
                        Author = "Sarah Chen",
                        Type = "Study Guide",
                        FileType = "PDF",
                        FileSizeMB = 2.4,
                        UploadedAgo = "3 days ago",
                        Downloads = 234,
                        FileUrl = "#"
                    },
                    new Resource {
                        Title = "JavaScript ES6 Cheat Sheet",
                        Author = "Mike Rodriguez",
                        Type = "Notes",
                        FileType = "PDF",
                        FileSizeMB = 1.8,
                        UploadedAgo = "1 week ago",
                        Downloads = 456,
                        FileUrl = "#"
                    }
                };
            }

            return View(resources);
        }
    }
}