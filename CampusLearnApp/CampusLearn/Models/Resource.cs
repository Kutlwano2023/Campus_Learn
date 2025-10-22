using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CampusLearn.Models
{
    public class Resource
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string Author { get; set; }

        [Required]
        public string Type { get; set; } // Notes, Study Guide, Research Paper, Video, Presentation

        [Required]
        public string FileType { get; set; } // PDF, DOCX, PPTX, MP4

        public double FileSizeMB { get; set; }

        [Required]
        public string FileUrl { get; set; }

        public string FileName { get; set; }

        public string ModuleId { get; set; } // Associated module
        public string TopicId { get; set; } // Associated topic

        public string UploadedBy { get; set; } // User ID who uploaded
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public int Downloads { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        // For display purposes
        [BsonIgnore]
        public string UploadedAgo => GetTimeAgo(UploadDate);

        private string GetTimeAgo(DateTime date)
        {
            var timeSpan = DateTime.UtcNow - date;
            if (timeSpan.TotalDays >= 365) return $"{(int)(timeSpan.TotalDays / 365)} years ago";
            if (timeSpan.TotalDays >= 30) return $"{(int)(timeSpan.TotalDays / 30)} months ago";
            if (timeSpan.TotalDays >= 1) return $"{(int)timeSpan.TotalDays} days ago";
            if (timeSpan.TotalHours >= 1) return $"{(int)timeSpan.TotalHours} hours ago";
            return $"{(int)timeSpan.TotalMinutes} minutes ago";
        }
    }
}