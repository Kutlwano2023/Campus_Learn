using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn.ViewModels
{
    public class UploadResourceViewModel
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public IFormFile File { get; set; }

        [Required]
        [Display(Name = "Resource Type")]
        public string Type { get; set; }

        public string ModuleId { get; set; }
        public string TopicId { get; set; }

        // For dropdown options
        public List<string> AvailableTypes => new List<string>
        {
            "Study Guide",
            "Lecture Notes",
            "Research Paper",
            "Video Lecture",
            "Presentation",
            "Assignment",
            "Tutorial"
        };
    }
}