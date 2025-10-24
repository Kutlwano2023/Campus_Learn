using CampusLearn.Models;
using CampusLearn.Services;
using CampusLearn.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLearn.Controllers
{
    [Authorize]
    public class ResourceController : Controller
    {
        private readonly MongoService _mongoService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IWebHostEnvironment _environment;

        public ResourceController(
            MongoService mongoService,
            IFileUploadService fileUploadService,
            IWebHostEnvironment environment)
        {
            _mongoService = mongoService;
            _fileUploadService = fileUploadService;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var resources = await _mongoService.Resources
                .Find(r => r.IsActive)
                .SortByDescending(r => r.UploadDate)
                .ToListAsync();

            return View(resources);
        }

        [HttpGet]
        [Authorize(Roles = "Tutor,Admin")]
        public IActionResult Upload()
        {
            var model = new UploadResourceViewModel();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Tutor,Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Upload(UploadResourceViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Run upload in a separate task to avoid blocking
            Task.Run(async () =>
            {
                try
                {
                    // Upload file
                    var fileUrl = await _fileUploadService.UploadFileAsync(model.File, "uploads/resources");

                    // Create resource
                    var resource = new Resource
                    {
                        Title = model.Title,
                        Description = model.Description,
                        Type = model.Type,
                        FileType = Path.GetExtension(model.File.FileName).ToUpper().TrimStart('.'),
                        FileSizeMB = Math.Round(model.File.Length / (1024.0 * 1024.0), 2),
                        FileUrl = fileUrl,
                        FileName = model.File.FileName,
                        ModuleId = model.ModuleId,
                        TopicId = model.TopicId,
                        UploadedBy = User.Identity.Name,
                        UploadDate = DateTime.UtcNow,
                        Author = User.Identity.Name
                    };

                    await _mongoService.Resources.InsertOneAsync(resource);
                }
                catch
                {
                    // You can log errors if needed, or handle them silently
                }
            });

            TempData["Success"] = "Resource upload started. It will appear shortly.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Tutor,Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var resource = await _mongoService.Resources.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (resource == null)
                return NotFound();

            await _fileUploadService.DeleteFileAsync(resource.FileUrl);

            var update = Builders<Resource>.Update.Set(r => r.IsActive, false);
            await _mongoService.Resources.UpdateOneAsync(r => r.Id == id, update);

            TempData["Success"] = "Resource deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Download(string id)
        {
            var resource = await _mongoService.Resources.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (resource == null)
                return NotFound();

            var update = Builders<Resource>.Update.Inc(r => r.Downloads, 1);
            await _mongoService.Resources.UpdateOneAsync(r => r.Id == id, update);

            var filePath = Path.Combine(_environment.WebRootPath, resource.FileUrl.TrimStart('/'));
            var contentType = _fileUploadService.GetContentType(resource.FileName);

            return PhysicalFile(filePath, contentType, resource.FileName);
        }

        public async Task<IActionResult> Search(string query, string type, string fileType)
        {
            var filter = Builders<Resource>.Filter.Where(r => r.IsActive);

            if (!string.IsNullOrEmpty(query))
            {
                filter &= Builders<Resource>.Filter.Or(
                    Builders<Resource>.Filter.Regex(r => r.Title, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<Resource>.Filter.Regex(r => r.Description, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<Resource>.Filter.Regex(r => r.Author, new MongoDB.Bson.BsonRegularExpression(query, "i"))
                );
            }

            if (!string.IsNullOrEmpty(type))
                filter &= Builders<Resource>.Filter.Eq(r => r.Type, type);

            if (!string.IsNullOrEmpty(fileType))
                filter &= Builders<Resource>.Filter.Eq(r => r.FileType, fileType);

            var resources = await _mongoService.Resources
                .Find(filter)
                .SortByDescending(r => r.UploadDate)
                .ToListAsync();

            return View("Index", resources);
        }
    }
}

