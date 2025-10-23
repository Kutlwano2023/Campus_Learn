using CampusLearn.Models;
using Microsoft.AspNetCore.Mvc;

namespace CampusLearn1.Controllers
{
    public class ResourcesController : Controller
    {
        // A static list to simulate a database that persists during the app's lifetime.
        private static List<Resource> _resources = new List<Resource>
        {
            new Resource
            {
                Id = "1",
                Title = "Advanced C# LINQ Techniques",
                Description = "A comprehensive guide to mastering Language-Integrated Query.",
                Author = "Jane Doe",
                Type = "Study Guide",
                FileType = "PDF",
                FileSizeMB = 2.5,
                FileUrl = "#",
                UploadDate = DateTime.UtcNow.AddDays(-10)
            },
            new Resource
            {
                Id = "2",
                Title = "ASP.NET Core MVC Project Setup",
                Description = "A 15-minute video walkthrough on setting up a new project from scratch.",
                Author = "John Smith",
                Type = "Video",
                FileType = "MP4",
                FileSizeMB = 78.2,
                FileUrl = "#",
                UploadDate = DateTime.UtcNow.AddHours(-5)
            }
        };

        // This action returns the main view with the list of resources.
        public IActionResult Index()
        {
            // Return the list sorted by the most recently uploaded first.
            return View(_resources.OrderByDescending(r => r.UploadDate).ToList());
        }

        // This action handles the HTTP POST request from the upload form.
        [HttpPost]
        public IActionResult Upload([FromForm] Resource resource)
        {
            // Check if the received data is valid based on model requirements.
            if (ModelState.IsValid)
            {
                // Simulate saving to a database by setting server-generated values.
                resource.Id = Guid.NewGuid().ToString(); // Assign a new unique ID.
                resource.UploadDate = DateTime.UtcNow; // Set the upload time to now.
                resource.FileUrl = "#"; // This would be the path to the saved file in a real app.

                // Add the new resource to our in-memory list.
                _resources.Add(resource);

                // Return an HTTP 200 OK status with the newly created resource data as JSON.
                return Ok(resource);
            }

            // If the model state is not valid, return a Bad Request with the validation errors.
            return BadRequest(ModelState);
        }
    }
}